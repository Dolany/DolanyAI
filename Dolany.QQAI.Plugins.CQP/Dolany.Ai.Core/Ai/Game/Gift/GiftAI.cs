using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;

namespace Dolany.Ai.Core.Ai.Game.Gift
{
    [AI(
        Name = "礼物",
        Description = "AI for Gifts.",
        Enable = true,
        PriorityLevel = 10,
        NeedManulOpen = true)]
    public class GiftAI : AIBase
    {
        [EnterCommand(ID = "GiftAI_MakeGift",
            Command = "制作礼物",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "制作指定的礼物",
            Syntax = "[礼物名]",
            Tag = "礼物功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true)]
        public bool MakeGift(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            // todo

            return true;
        }
    }
}
