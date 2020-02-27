using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;

namespace Dolany.WorldLine.Standard.Ai.Game.Advanture
{
    public class AdvantureAi : AIBase
    {
        public override string AIName { get; set; } = "冒险对决";

        public override string Description { get; set; } = "AI for Advanture Fight.";

        public override AIPriority PriorityLevel { get;} = AIPriority.Normal;

        public override bool NeedManualOpeon { get; } = true;

        public BindAiSvc BindAiSvc { get; set; }
        public AdvGameSvc AdvGameSvc { get; set; }

        [EnterCommand(ID = "AdvantureAi_AdvantureAgainst",
            Command = "冒险对决",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "和一名成员进行冒险对决",
            Syntax = "[@QQ号]",
            SyntaxChecker = "At",
            Tag = "游戏功能",
            IsPrivateAvailable = false,
            DailyLimit = 1,
            TestingDailyLimit = 2)]
        public bool AdvantureAgainst(MsgInformationEx MsgDTO, object[] param)
        {
            var aimNum = (long) param[0];
            if (MsgDTO.FromQQ == aimNum)
            {
                MsgSender.PushMsg(MsgDTO, "你无法跟自己对决！");
                return false;
            }

            if (BindAiSvc.AllAiNums.Contains(aimNum))
            {
                MsgSender.PushMsg(MsgDTO, "鱼唇的人类，你无法挑战ai的威严！");
                return false;
            }

            if (!AdvGameSvc.CheckGroup(MsgDTO.FromGroup))
            {
                MsgSender.PushMsg(MsgDTO, "本群正在进行一场对决，请稍后再试！");
                return false;
            }

            if (!WaiterSvc.WaitForConfirm(MsgDTO.FromGroup, aimNum,
                $"{CodeApi.Code_At(MsgDTO.FromQQ)} 正在向你发起一场冒险对决，是否接受？",MsgDTO.BindAi, 10))
            {
                MsgSender.PushMsg(MsgDTO, "对决取消！");
                return false;
            }

            if (!AdvGameSvc.CheckPlayer(aimNum))
            {
                MsgSender.PushMsg(MsgDTO, "你的对手正在进行一场对决，请稍后再试！");
                return false;
            }

            if (!AdvGameSvc.CheckPlayer(MsgDTO.FromQQ))
            {
                MsgSender.PushMsg(MsgDTO, "你正在进行一场对决，请稍后再试！");
                return false;
            }

            AdvGameSvc.GameStart(MsgDTO.FromGroup, MsgDTO.FromQQ, aimNum, 1, MsgDTO.BindAi);
            return true;
        }
    }
}
