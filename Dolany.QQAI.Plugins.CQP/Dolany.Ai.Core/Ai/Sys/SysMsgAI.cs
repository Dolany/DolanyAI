﻿namespace Dolany.Ai.Core.Ai.Sys
{
    using Base;
    using Common;
    using Model;

    using static Common.Utility;

    [AI(
        Name = nameof(SysMsgAI),
        Description = "AI for System msg consoling.",
        IsAvailable = true,
        PriorityLevel = 20)]
    public class SysMsgAI : AIBase
    {
        public SysMsgAI()
        {
            RuntimeLogger.Log("SysMsgAI started.");
        }

        public override void Work()
        {
        }

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
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
            return false;
        }

        private static void ParseRollBack(MsgInformationEx MsgDTO)
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
