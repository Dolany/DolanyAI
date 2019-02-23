using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Model;

namespace Dolany.Ai.Core.Ai.Game.TouhouCard
{
    [AI(
        Name = "东方卡牌战争",
        Description = "AI for Touhou Spell Card War.",
        Enable = false,
        PriorityLevel = 10,
        NeedManulOpen = true)]
    public class SpellCardAI : AIBase
    {
        [EnterCommand(
            Command = "合成卡牌",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "合成一包卡牌",
            Syntax = "[卡牌名]",
            SyntaxChecker = "Word",
            Tag = "东方卡牌战争功能",
            IsPrivateAvailable = true,
            DailyLimit = 3,
            TestingDailyLimit = 5)]
        public bool CombineCard(MsgInformationEx MsgDTO, object[] param)
        {
            // todo

            return true;
        }

        [EnterCommand(
            Command = "我的卡牌",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看自己拥有的卡牌",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "东方卡牌战争功能",
            IsPrivateAvailable = true)]
        public bool ViewCards(MsgInformationEx MsgDTO, object[] param)
        {
            // todo

            return true;
        }
    }
}
