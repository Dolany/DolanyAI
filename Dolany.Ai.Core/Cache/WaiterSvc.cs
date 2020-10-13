using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Common;
using Dolany.Database.Ai;
using Dolany.UtilityTool;
using Newtonsoft.Json;

namespace Dolany.Ai.Core.Cache
{
    /// <summary>
    /// 等待服务
    /// </summary>
    public class WaiterSvc : IDependency
    {
        private readonly object _lockObj = new object();

        private readonly Dictionary<long, List<WaiterUnit>> UnitsDic = new Dictionary<long, List<WaiterUnit>>();

        public GroupSettingSvc GroupSettingSvc { get; set; }
        public BindAiSvc BindAiSvc { get; set; }
        public QQNumReflectSvc QqNumReflectSvc { get; set; }
        public CrossWorldAiSvc CrossWorldAiSvc { get; set; }

        public void Listen()
        {
            Global.CommandInfoService.StartReceive<MsgInformation>(ListenCallBack);
        }

        private void ListenCallBack(MsgInformation info)
        {
            var source = "未知";
            if (info.FromGroup == 0)
            {
                source = "私聊";
            }
            else
            {
                var groupSetting = GroupSettingSvc[info.FromGroup];
                if (groupSetting != null)
                {
                    source = groupSetting.Name;
                }
            }

            if (info.FromGroup != 0)
            {
                var setting = GroupSettingSvc[info.FromGroup];
                if (setting == null || setting.BindAi != info.BindAi)
                {
                    return;
                }
            }

            var msg = $"[Info] {info.BindAi} {source} {QqNumReflectSvc[info.FromQQ]} {info.Msg}";
            Global.MsgPublish(msg);

            if (BindAiSvc[info.FromQQ] != null)
            {
                return;
            }

            switch (info.Information)
            {
                case InformationType.Message:
                case InformationType.CommandBack:
                    WaiterUnit waitUnit = null;
                    lock (_lockObj)
                    {
                        if (UnitsDic.ContainsKey(info.FromGroup) && !UnitsDic[info.FromGroup].IsNullOrEmpty())
                        {
                            waitUnit = UnitsDic[info.FromGroup].FirstOrDefault(u => u.JudgePredicate(info));
                        }
                    }

                    if (waitUnit == null)
                    {
                        CrossWorldAiSvc.OnMsgReceived(info);
                        break;
                    }

                    waitUnit.ResultInfos.Add(info);
                    waitUnit.Signal.Set();
                    AIAnalyzer.AddCommandCount(new CmdRec()
                    {
                        FunctionalAi = "Waiter",
                        Command = "WaiterCallBack",
                        GroupNum = info.FromGroup,
                        BindAi = info.BindAi
                    });

                    break;
                case InformationType.AuthCode:
                    break;
                case InformationType.Error:
                    AIAnalyzer.AddError(info.Msg);
                    break;
                case InformationType.ReceiveMoney:
                    CrossWorldAiSvc.OnMoneyReceived(JsonConvert.DeserializeObject<ChargeModel>(info.Msg));
                    break;
                case InformationType.GroupMemberIncrease:
                case InformationType.GroupMemberDecrease:
                    CrossWorldAiSvc.OnGroupMemberChanged(JsonConvert.DeserializeObject<GroupMemberChangeModel>(info.Msg));
                    break;
                case InformationType.ConnectStateChange:
                    var bindai = BindAiSvc[info.BindAi];
                    bindai.IsConnected = bool.Parse(info.Msg);
                    break;
            }
        }

        public MsgInformation WaitForInformation(MsgInformationEx MsgDTO, string msg, Predicate<MsgInformation> judgeFunc, int timeout = 7, bool isNeedAt = false)
        {
            var signal = new AutoResetEvent(false);
            var unit = new WaiterUnit {JudgePredicate = judgeFunc, Signal = signal};
            lock (_lockObj)
            {
                if (UnitsDic.ContainsKey(MsgDTO.FromGroup))
                {
                    UnitsDic[MsgDTO.FromGroup].Add(unit);
                }
                else
                {
                    UnitsDic.Add(MsgDTO.FromGroup, new List<WaiterUnit>() {unit});
                }
            }

            MsgSender.PushMsg(MsgDTO, msg, isNeedAt);
            signal.WaitOne(timeout * 1000);

            lock (_lockObj)
            {
                unit = UnitsDic[MsgDTO.FromGroup].FirstOrDefault(u => u.Id == unit.Id);
                UnitsDic[MsgDTO.FromGroup].Remove(unit);
            }

            return unit?.ResultInfos.FirstOrDefault();
        }

        public MsgInformation WaitForInformation(MsgCommand command, Predicate<MsgInformation> judgeFunc, int timeout = 7)
        {
            var signal = new AutoResetEvent(false);
            var unit = new WaiterUnit {JudgePredicate = judgeFunc, Signal = signal};
            lock (_lockObj)
            {
                if (UnitsDic.ContainsKey(command.ToGroup))
                {
                    UnitsDic[command.ToGroup].Add(unit);
                }
                else
                {
                    UnitsDic.Add(command.ToGroup, new List<WaiterUnit>() {unit});
                }
            }

            MsgSender.PushMsg(command);
            signal.WaitOne(timeout * 1000);

            lock (_lockObj)
            {
                unit = UnitsDic[command.ToGroup].FirstOrDefault(u => u.Id == unit.Id);
                UnitsDic[command.ToGroup].Remove(unit);
            }

            return unit?.ResultInfos.FirstOrDefault();
        }

        public MsgInformation WaitForRelationId(MsgCommand command, int timeout = 7)
        {
            return WaitForInformation(command, information => information.RelationId == command.Id, timeout);
        }

        public bool WaitForConfirm(MsgInformationEx MsgDTO, string msg, int timeout = 7, string ConfirmTxt = "确认", string CancelTxt = "取消")
        {
            msg += $"\r1：{ConfirmTxt}，2：{CancelTxt}";
            var response = WaitForInformation(MsgDTO, msg,
                information => information.FromGroup == MsgDTO.FromGroup && information.FromQQ == MsgDTO.FromQQ && int.TryParse(information.Msg, out var i) &&
                               (i == 1 || i == 2), timeout, true);
            return response != null && int.TryParse(response.Msg, out var ri) && ri == 1;
        }

        public bool WaitForConfirm_Gold(MsgInformationEx MsgDTO, int golds, int timeout = 7)
        {
            return WaitForConfirm(MsgDTO, $"此操作将花费 {golds.CurencyFormat()}，是否继续？", timeout);
        }

        public bool WaitForConfirm(long ToGroup, long ToQQ, string msg, string BindAi, int timeout = 7, string ConfirmTxt = "确认", string CancelTxt = "取消")
        {
            return WaitForConfirm(new MsgInformationEx {FromQQ = ToQQ, FromGroup = ToGroup, Type = ToGroup == 0 ? MsgType.Private : MsgType.Group, BindAi = BindAi},
                msg, timeout, ConfirmTxt, CancelTxt);
        }

        public int WaitForNum(long ToGroup, long ToQQ, string msg, Predicate<int> predicate, string BindAi, int timeout = 10, bool isNeedAt = true)
        {
            var msgInfo = WaitForInformation(
                new MsgInformationEx() {FromGroup = ToGroup, FromQQ = ToQQ, BindAi = BindAi, Type = ToGroup == 0 ? MsgType.Private : MsgType.Group}, msg,
                info => info.FromGroup == ToGroup && info.FromQQ == ToQQ && int.TryParse(info.Msg, out var res) && predicate(res), timeout,
                isNeedAt && ToGroup != 0);
            if (msgInfo != null && int.TryParse(msgInfo.Msg, out var aimr))
            {
                return aimr;
            }

            return -1;
        }

        public int WaitForOptions(long ToGroup, long ToQQ, string preMsg, string[] options, string BindAi)
        {
            var msg = $"{preMsg}\r\n{string.Join("\r\n", options.Select((option, idx) => $"{idx + 1}:{option}"))}";
            var result = WaitForNum(ToGroup, ToQQ, msg, i => i > 0 && i <= options.Length, BindAi);
            return result < 0 ? result : result - 1;
        }
    }

    /// <summary>
    /// 等待单元
    /// </summary>
    public class WaiterUnit
    {
        /// <summary>
        /// 等待单元ID
        /// </summary>
        public string Id { get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 判断表达式
        /// </summary>
        public Predicate<MsgInformation> JudgePredicate { get; set; }

        /// <summary>
        /// 阻塞信号量
        /// </summary>
        public AutoResetEvent Signal { get; set; }

        /// <summary>
        /// 等待结果集
        /// </summary>
        public List<MsgInformation> ResultInfos { get; } = new List<MsgInformation>();
    }
}
