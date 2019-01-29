namespace Dolany.Ai.Core.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Common;
    using Dolany.Database.Ai;

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

        public MsgInformation WaitForInformation(MsgCommand sendMsg, Predicate<MsgInformation> judgeFunc, int timeout = 5000)
        {
            var signal = new AutoResetEvent(false);
            var unit = new WaiterUnit {JudgePredicate = judgeFunc, Signal = signal};
            lock (_lockObj)
            {
                Units.Add(unit);
            }

            MsgSender.Instance.PushMsg(sendMsg);
            signal.WaitOne(timeout);

            lock (_lockObj)
            {
                unit = Units.FirstOrDefault(u => u.Id == unit.Id);
                Units.Remove(unit);
            }

            return unit?.ResultInfos.FirstOrDefault();
        }

        public IEnumerable<MsgInformation> WaitForInformations(MsgCommand sendMsg, IEnumerable<Predicate<MsgInformation>> judgeFuncs, int timeout = 5000)
        {
            MsgSender.Instance.PushMsg(sendMsg);

            var tasks = judgeFuncs.Select(func => Task.Factory.StartNew(() =>
            {
                var signal = new AutoResetEvent(false);
                var unit = new WaiterUnit {JudgePredicate = func, Signal = signal};
                lock (_lockObj)
                {
                    Units.Add(unit);
                }

                signal.WaitOne(timeout);

                lock (_lockObj)
                {
                    unit = Units.FirstOrDefault(u => u.Id == unit.Id);
                    Units.Remove(unit);
                }

                return unit?.ResultInfos.FirstOrDefault();
            })).ToArray();
            // ReSharper disable once CoVariantArrayConversion
            Task.WaitAll(tasks, timeout);

            return tasks.Select(task => task.Result);
        }

        public List<MsgInformation> WaitWhile(MsgCommand sendMsg, Predicate<MsgInformation> judgeFunc, int timeout = 5000)
        {
            var signal = new AutoResetEvent(false);
            var unit = new WaiterUnit { JudgePredicate = judgeFunc, Signal = signal, Type = WaiterUnitType.Multi};
            lock (_lockObj)
            {
                Units.Add(unit);
            }

            MsgSender.Instance.PushMsg(sendMsg);
            signal.WaitOne(timeout);

            lock (_lockObj)
            {
                unit = Units.FirstOrDefault(u => u.Id == unit.Id);
                Units.Remove(unit);
            }

            return unit?.ResultInfos;
        }

        public MsgInformation WaitForRelationId(MsgCommand sendMsg, int timeout = 5000)
        {
            return WaitForInformation(sendMsg, information => information.RelationId == sendMsg.Id, timeout);
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
