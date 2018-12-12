namespace Dolany.Ai.Core.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Timers;

    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Db;

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
            JobScheduler.Instance.Add(500, ListenCallBack);
        }

        private void ListenCallBack(object sender, ElapsedEventArgs e)
        {
            using (var db = new AIDatabase())
            {
                var infos = db.MsgInformation.ToList();
                foreach (var info in infos)
                {
                    var msg =
                        $"[Information] {info.Information} {info.FromGroup} {info.FromQQ} {info.RelationId} {info.Msg}";
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
                            }
                            else
                            {
                                waitUnit.ResultInfo = info;
                                waitUnit.Signal.Set();
                            }

                            break;
                        case AiInformation.AuthCode:
                            Global.AuthCode = info.Msg;
                            // Utility.SendMsgToDeveloper($"AuthCode:{Global.AuthCode}");
                            break;
                    }

                    db.MsgInformation.Remove(info);
                }

                db.SaveChanges();
            }
        }

        public MsgInformation WaitForInformation(
            MsgCommand sendMsg,
            Predicate<MsgInformation> judgeFunc,
            int timeout = 5000)
        {
            var signal = new AutoResetEvent(false);
            var unit = new WaiterUnit { JudgePredicate = judgeFunc, Signal = signal };
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
            return unit?.ResultInfo;
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

        public MsgInformation ResultInfo { get; set; }
    }
}
