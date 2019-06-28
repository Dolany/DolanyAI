using System.Threading;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.SyntaxChecker;

namespace Dolany.Ai.Core.Ai.Repeater
{
    [AI(Name = "随机复读",
        Description = "AI for Repeating Random words.",
        Enable = true,
        PriorityLevel = 1,
        NeedManulOpen = true)]
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

            var setting = GroupSettingMgr.Instance[MsgDTO.FromGroup];
            if (!setting.HasFunction("随机复读"))
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
