﻿using Dolany.Ai.Common.Models;
using Dolany.Ai.Doremi.Ai.Game.Xiuxian;
using Dolany.Ai.Doremi.Base;
using Dolany.Ai.Doremi.Cache;
using Dolany.Ai.Doremi.OnlineStore;
using Dolany.Ai.Doremi.Xiuxian;

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
            IsPrivateAvailable = false)]
        public bool MyStatus(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            var level = LevelMgr.Instance.GetByLevel(osPerson.Level);
            var exp = MsgCounterSvc.Get(MsgDTO.FromQQ);

            var msg = $"等级：{level.Name}\r" + $"经验值：{exp}/{level.Exp}{(exp >= level.Exp ? "(可渡劫)" : "")}\r" + $"金币：{osPerson.Golds}";

            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }
    }
}
