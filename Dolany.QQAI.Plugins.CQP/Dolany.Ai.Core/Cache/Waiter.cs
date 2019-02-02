using Dolany.Ai.Common;
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

        private readonly List<WaiterUnit> Units = new List<WaiterUnit>();

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
            var msg = $"[Information] {info.Information} {info.FromGroup} {info.FromQQ} {info.Msg}";
            AIMgr.Instance.MessagePublish(msg);

            switch (info.Information)
            {
                case AiInformation.Message:
                case AiInformation.CommandBack:
                    WaiterUnit waitUnit;
                    lock (_lockObj)
                    {
                        waitUnit = Units.FirstOrDefault(u => u.JudgePredicate(info));
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

                    break;
                case AiInformation.AuthCode:
                    Global.AuthCode = info.Msg;
                    break;
                case AiInformation.Error:
                    Sys_ErrorCount.Plus(info.Msg);
                    break;
            }
        }

        public MsgInformation WaitForInformation(MsgInformationEx MsgDTO, string msg, Predicate<MsgInformation> judgeFunc, int timeout = 5)
        {
            var signal = new AutoResetEvent(false);
            var unit = new WaiterUnit {JudgePredicate = judgeFunc, Signal = signal};
            lock (_lockObj)
            {
                Units.Add(unit);
            }

            MsgSender.Instance.PushMsg(MsgDTO, msg);
            signal.WaitOne(timeout * 1000);

            lock (_lockObj)
            {
                unit = Units.FirstOrDefault(u => u.Id == unit.Id);
                Units.Remove(unit);
            }

            return unit?.ResultInfos.FirstOrDefault();
        }

        public MsgInformation WaitForInformation(MsgCommand command, Predicate<MsgInformation> judgeFunc, int timeout = 5)
        {
            var signal = new AutoResetEvent(false);
            var unit = new WaiterUnit {JudgePredicate = judgeFunc, Signal = signal};
            lock (_lockObj)
            {
                Units.Add(unit);
            }

            MsgSender.Instance.PushMsg(command);
            signal.WaitOne(timeout * 1000);

            lock (_lockObj)
            {
                unit = Units.FirstOrDefault(u => u.Id == unit.Id);
                Units.Remove(unit);
            }

            return unit?.ResultInfos.FirstOrDefault();
        }

        public IEnumerable<MsgInformation> WaitForInformations(MsgInformationEx MsgDTO, string msg, IEnumerable<Predicate<MsgInformation>> judgeFuncs, int timeout = 5)
        {
            MsgSender.Instance.PushMsg(MsgDTO, msg);

            var tasks = judgeFuncs.Select(func => Task.Factory.StartNew(() =>
            {
                var signal = new AutoResetEvent(false);
                var unit = new WaiterUnit {JudgePredicate = func, Signal = signal};
                lock (_lockObj)
                {
                    Units.Add(unit);
                }

                signal.WaitOne(timeout * 1000);

                lock (_lockObj)
                {
                    unit = Units.FirstOrDefault(u => u.Id == unit.Id);
                    Units.Remove(unit);
                }

                return unit?.ResultInfos.FirstOrDefault();
            })).ToArray();
            // ReSharper disable once CoVariantArrayConversion
            Task.WaitAll(tasks, timeout * 1000);

            return tasks.Select(task => task.Result);
        }

        public List<MsgInformation> WaitWhile(MsgInformationEx MsgDTO, string msg, Predicate<MsgInformation> judgeFunc, int timeout)
        {
            var signal = new AutoResetEvent(false);
            var unit = new WaiterUnit { JudgePredicate = judgeFunc, Signal = signal, Type = WaiterUnitType.Multi};
            lock (_lockObj)
            {
                Units.Add(unit);
            }

            MsgSender.Instance.PushMsg(MsgDTO, msg);
            signal.WaitOne(timeout * 1000);

            lock (_lockObj)
            {
                unit = Units.FirstOrDefault(u => u.Id == unit.Id);
                Units.Remove(unit);
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

        public bool WaitForConfirm(MsgInformationEx MsgDTO, string msg, int timeout = 5)
        {
            msg += "\r1：确认，2：取消";
            var response = WaitForInformation(MsgDTO, msg, information => int.TryParse(information.Msg, out var i) && (i == 1 || i == 2), timeout);
            return response != null && int.TryParse(response.Msg, out var ri) && ri == 1;
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
