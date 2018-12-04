using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Cache
{
    using System.Collections.Immutable;
    using System.Threading;

    using Dolany.Ai.Core.Db;
    using Dolany.Ai.Core.DTO;

    public class Waiter
    {
        private ImmutableList<WaiterUnit> Units = ImmutableList.Create<WaiterUnit>();

        public static Waiter Instance { get; } = new Waiter();

        private Waiter()
        {

        }

        public MsgInformation WaitForInformation(SendMsgDTO sendMsg, Predicate<MsgInformation> judgeFunc)
        {
            var signal = new AutoResetEvent(false);
            var unit = new WaiterUnit { JudgePredicate = judgeFunc, Signal = signal };
            Units.Add(unit);
            MsgSender.Instance.PushMsg(sendMsg);
            signal.WaitOne();

            return unit.ResultInfo;
        }
    }

    public class WaiterUnit
    {
        public Predicate<MsgInformation> JudgePredicate { get; set; }
        public AutoResetEvent Signal { get; set; }
        public MsgInformation ResultInfo { get; set; }
    }
}
