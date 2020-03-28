using System.Threading;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Doremi.Base;
using Dolany.Ai.Doremi.Cache;
using Dolany.Ai.Doremi.Common;
using Dolany.Ai.Doremi.SyntaxChecker;

namespace Dolany.Ai.Doremi.Ai.Repeater
{
    [AI(Name = "随机复读",
        Description = "AI for Repeating Random words.",
        Enable = true,
        PriorityLevel = 1,
        NeedManulOpen = true,
        BindAi = "Doremi")]
    public class RepeatorAI : AIBase
    {
        private const long RepeatLimit = 30;

        private long CurCount;

        private const int SleepTime = 2000;

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            if (MsgDTO.Type == MsgType.Private)
            {
                return false;
            }

            var atChecker = new AtChecker();
            if (atChecker.Check(MsgDTO.FullMsg, out _))
            {
                return false;
            }

            CurCount++;
            if (CurCount < RepeatLimit)
            {
                return false;
            }

            CurCount %= RepeatLimit;
            Repeat(MsgDTO);
            AIAnalyzer.AddCommandCount(new CommandAnalyzeDTO()
            {
                Ai = AIAttr.Name,
                Command = "RepeatorOverride",
                GroupNum = MsgDTO.FromGroup,
                BindAi = MsgDTO.BindAi
            });
            return true;
        }

        private static void Repeat(MsgInformationEx MsgDTO)
        {
            Thread.Sleep(SleepTime);

            MsgSender.PushMsg(MsgDTO, MsgDTO.FullMsg);
        }
    }
}
