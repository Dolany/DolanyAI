using Dolany.Ai.Reborn.DolanyAI.Base;
using Dolany.Ai.Reborn.DolanyAI.Common;
using Dolany.Ai.Reborn.DolanyAI.DTO;
using static Dolany.Ai.Reborn.DolanyAI.Common.Utility;

namespace Dolany.Ai.Reborn.DolanyAI.Ai.Sys.SysMsg
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

            if (MsgDTO.FromQQ != SysMsgNumber)
            {
                return false;
            }

            ParseRollBack(MsgDTO);
            return true;
        }

        private static void ParseRollBack(ReceivedMsgDTO MsgDTO)
        {
            if (!MsgDTO.FullMsg.Contains("撤回"))
            {
                return;
            }

            var picName = ParsePicName(MsgDTO.FullMsg);
            if (string.IsNullOrEmpty(picName))
            {
                return;
            }

            RemovePicCache(picName);
        }
    }
}
