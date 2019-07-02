using Dolany.Ai.Common.Models;
using Dolany.Ai.Doremi.Base;

namespace Dolany.Ai.Doremi.Ai.Game.Shopping
{
    [AI(Name = "商店",
        Description = "AI for shopping.",
        Enable = true,
        PriorityLevel = 10,
        NeedManulOpen = true,
        BindAi = "DoreFun")]
    public class ShoppingAI : AIBase
    {
        [EnterCommand(ID = "ShoppingAI_MyStatus",
            Command = "我的状态",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取自身当前状态",
            Syntax = "",
            Tag = "修仙功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true)]
        public bool MyStatus(MsgInformationEx MsgDTO, object[] param)
        {
            // todo
            return true;
        }
    }
}
