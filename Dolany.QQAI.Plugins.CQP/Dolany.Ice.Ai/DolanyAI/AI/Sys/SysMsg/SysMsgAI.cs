namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(SysMsgAI),
        Description = "AI for System msg consoling.",
        IsAvailable = true,
        PriorityLevel = 20
    )]
    public class SysMsgAI : AIBase
    {
        public SysMsgAI()
        {
            RuntimeLogger.Log("SysMsgAI started.");
        }

        public override void Work()
        {
        }

        public override bool OnMsgReceived(ReceivedMsgDTO MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            if (MsgDTO.FromQQ != Utility.SysMsgNumber)
            {
                return false;
            }

            // TODO
            return true;
        }
    }
}