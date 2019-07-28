﻿using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Doremi.Ai.Game.Xiuxian;
using Dolany.Ai.Doremi.API;
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

            var msg = $"等级：{level.Name}\r" +
                      $"经验值：{exp}/{level.Exp}{(exp >= level.Exp ? "(可渡劫)" : "")}\r" +
                      $"{Emoji.心}:{level.HP}\r" +
                      $"{Emoji.剑}:{level.Atk}\r" +
                      $"金币：{osPerson.Golds}";

            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }

        [EnterCommand(ID = "ShoppingAI_Buy",
            Command = "购买",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "购买某件物品",
            Syntax = "[物品名]",
            Tag = "修仙功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = false)]
        public bool Buy(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            if (RandShopper.Instance.SellingGoods.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "商店尚未营业！", true);
                return false;
            }

            if (!RandShopper.Instance.SellingGoods.Contains(name))
            {
                MsgSender.PushMsg(MsgDTO, "此商品未在商店中出售！", true);
                return false;
            }

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            var armerModel = ArmerMgr.Instance[name];
            if (osPerson.Golds < armerModel.Price)
            {
                MsgSender.PushMsg(MsgDTO, $"你持有的金币不足以购买此物品({osPerson.Golds}/{armerModel.Price})", true);
                return false;
            }

            if (!Waiter.Instance.WaitForConfirm_Gold(MsgDTO, armerModel.Price))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            osPerson.Golds -= armerModel.Price;
            var paRec = PersonArmerRecord.Get(MsgDTO.FromQQ);
            paRec.ArmerGet(name);
            paRec.Update();
            osPerson.Update();

            MsgSender.PushMsg(MsgDTO, "购买成功！");
            return true;
        }

        [EnterCommand(ID = "ShoppingAI_MyArmers",
            Command = "我的装备",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取当前持有的装备",
            Syntax = "",
            Tag = "修仙功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool MyArmers(MsgInformationEx MsgDTO, object[] param)
        {
            var paRec = PersonArmerRecord.Get(MsgDTO.FromQQ);
            if (paRec.Armers.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "你尚未拥有任何装备！", true);
                return true;
            }

            var msg = string.Join(", ", paRec.Armers.Select(a => $"{a.Key}*{a.Value}"));

            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }
    }
}
