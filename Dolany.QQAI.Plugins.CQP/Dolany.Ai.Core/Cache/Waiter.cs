﻿using Dolany.Ai.Common;
using Dolany.Ai.Core.Model;

namespace Dolany.Ai.Core.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Common;

    public class Waiter
    {
        private readonly object _lockObj = new object();

        private readonly Dictionary<long, List<WaiterUnit>> UnitsDic = new Dictionary<long, List<WaiterUnit>>();

        public static Waiter Instance { get; } = new Waiter();

        private Waiter()
        {
        }

        public void Listen()
        {
            Global.CommandInfoService.StartReceive<MsgInformation>(ListenCallBack);
        }

        private void ListenCallBack(MsgInformation info)
        {
            var msg = $"[Information] {info.Information} " +
                      $"{(info.FromGroup == 0 ? "私聊" : AIMgr.Instance.AllGroupsDic[info.FromGroup])} " +
                      $"{info.FromQQ} " +
                      $"{info.Msg}";
            AIMgr.Instance.MessagePublish(msg);

            switch (info.Information)
            {
                case AiInformation.Message:
                case AiInformation.CommandBack:
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
                        AIMgr.Instance.OnMsgReceived(info);
                        break;
                    }

                    waitUnit.ResultInfos.Add(info);
                    if(waitUnit.Type == WaiterUnitType.Single)
                    {
                        waitUnit.Signal.Set();
                    }
                    AIAnalyzer.AddCommandCount(new CommandAnalyzeDTO()
                    {
                        Ai = "Waiter",
                        Command = "WaiterCallBack",
                        GroupNum = info.FromGroup
                    });

                    break;
                case AiInformation.AuthCode:
                    Global.AuthCode = info.Msg;
                    break;
                case AiInformation.Error:
                    AIAnalyzer.AddError(info.Msg);
                    break;
            }
        }

        public MsgInformation WaitForInformation(MsgInformationEx MsgDTO, string msg, Predicate<MsgInformation> judgeFunc, int timeout = 5, bool isNeedAt = false)
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

        public MsgInformation WaitForInformation(MsgCommand command, Predicate<MsgInformation> judgeFunc, int timeout = 5)
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

        public IEnumerable<MsgInformation> WaitForInformations(MsgInformationEx MsgDTO, string msg, IEnumerable<Predicate<MsgInformation>> judgeFuncs, int timeout = 5)
        {
            MsgSender.PushMsg(MsgDTO, msg);

            var tasks = judgeFuncs.Select(func => Task.Factory.StartNew(() =>
            {
                var signal = new AutoResetEvent(false);
                var unit = new WaiterUnit {JudgePredicate = func, Signal = signal};
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

                signal.WaitOne(timeout * 1000);

                lock (_lockObj)
                {
                    unit = UnitsDic[MsgDTO.FromGroup].FirstOrDefault(u => u.Id == unit.Id);
                    UnitsDic[MsgDTO.FromGroup].Remove(unit);
                }

                return unit?.ResultInfos.FirstOrDefault();
            })).ToArray();
            Task.WaitAll(tasks, timeout * 1000);

            return tasks.Select(task => task.Result);
        }

        public List<MsgInformation> WaitWhile(MsgInformationEx MsgDTO, string msg, Predicate<MsgInformation> judgeFunc, int timeout)
        {
            var signal = new AutoResetEvent(false);
            var unit = new WaiterUnit { JudgePredicate = judgeFunc, Signal = signal, Type = WaiterUnitType.Multi};
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

            MsgSender.PushMsg(MsgDTO, msg);
            signal.WaitOne(timeout * 1000);

            lock (_lockObj)
            {
                unit = UnitsDic[MsgDTO.FromGroup].FirstOrDefault(u => u.Id == unit.Id);
                UnitsDic[MsgDTO.FromGroup].Remove(unit);
            }

            return unit?.ResultInfos;
        }

        public MsgInformation WaitForRelationId(MsgInformationEx MsgDTO, string msg, int timeout = 5)
        {
            return WaitForInformation(MsgDTO, msg, information => information.RelationId == MsgDTO.Id, timeout);
        }

        public MsgInformation WaitForRelationId(MsgCommand command, int timeout = 5)
        {
            return WaitForInformation(command, information => information.RelationId == command.Id, timeout);
        }

        public bool WaitForConfirm(MsgInformationEx MsgDTO, string msg, int timeout = 5, string ConfirmTxt = "确认", string CancelTxt = "取消")
        {
            msg += $"\r1：{ConfirmTxt}，2：{CancelTxt}";
            var response = WaitForInformation(MsgDTO, msg,
                information => information.FromGroup == MsgDTO.FromGroup && information.FromQQ == MsgDTO.FromQQ && int.TryParse(information.Msg, out var i) &&
                               (i == 1 || i == 2), timeout, true);
            return response != null && int.TryParse(response.Msg, out var ri) && ri == 1;
        }

        public bool WaitForConfirm(MsgInformationEx MsgDTO, int golds, int timeout = 5)
        {
            return WaitForConfirm(MsgDTO, $"此操作将花费 {golds} 金币，是否继续？", timeout);
        }

        public bool WaitForConfirm(long ToGroup, long ToQQ, string msg, int timeout = 5, string ConfirmTxt = "确认", string CancelTxt = "取消")
        {
            return WaitForConfirm(new MsgInformationEx {FromQQ = ToQQ, FromGroup = ToGroup, Type = MsgType.Group}, msg, timeout, ConfirmTxt, CancelTxt);
        }

        public int WaitForNum(long ToGroup, long ToQQ, string msg, Predicate<int> predicate)
        {
            var msgInfo = WaitForInformation(new MsgInformationEx()
                {
                    FromGroup = ToGroup,
                    FromQQ = ToQQ
                }, msg, info => info.FromGroup == ToGroup
                                && info.FromQQ == ToQQ
                                && int.TryParse(info.Msg, out var res)
                                && predicate(res), 7, ToQQ != 0);
            if (msgInfo != null && int.TryParse(msgInfo.Msg, out var aimr))
            {
                return aimr;
            }

            return -1;
        }
    }

    public class WaiterUnit
    {
        public string Id { get; } = Guid.NewGuid().ToString();

        public Predicate<MsgInformation> JudgePredicate { get; set; }

        public AutoResetEvent Signal { get; set; }

        public List<MsgInformation> ResultInfos { get; set; } = new List<MsgInformation>();

        public WaiterUnitType Type { get; set; } = WaiterUnitType.Single;
    }

    public enum WaiterUnitType
    {
        Single,
        Multi
    }
}
