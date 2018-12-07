using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Cache
{
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Timers;

    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Db;

    public class Waiter
    {
        private ImmutableList<WaiterUnit> Units = ImmutableList.Create<WaiterUnit>();

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
                    var waitUnit = this.Units.FirstOrDefault(u => u.JudgePredicate(info));
                    if (waitUnit == null)
                    {
                        AIMgr.Instance.OnMsgReceived(info);
                    }
                    else
                    {
                        waitUnit.Signal.Set();
                        Units.Remove(waitUnit);
                    }

                    db.MsgInformation.Remove(info);
                }

                db.SaveChanges();
            }
        }

        public MsgInformation WaitForInformation(MsgCommand sendMsg, Predicate<MsgInformation> judgeFunc, int timeout = 5000)
        {
            var signal = new AutoResetEvent(false);
            var unit = new WaiterUnit { JudgePredicate = judgeFunc, Signal = signal };
            Units.Add(unit);
            MsgSender.Instance.PushMsg(sendMsg);
            signal.WaitOne(timeout);

            return unit.ResultInfo;
        }

        public MsgInformation WaitForRelationId(MsgCommand sendMsg, int timeout = 5000)
        {
            return WaitForInformation(sendMsg, information => information.RelationId == sendMsg.Id, timeout);
        }
    }

    public class WaiterUnit
    {
        public Predicate<MsgInformation> JudgePredicate { get; set; }
        public AutoResetEvent Signal { get; set; }
        public MsgInformation ResultInfo { get; set; }
    }
}
