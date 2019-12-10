﻿using System;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Vip.VipArmers
{
    public class 迷之匣 : IVipArmer
    {
        public string Name { get; set; } = "迷之匣";
        public string Description { get; set; } = "随机获得一个正在商店售卖的商品，每天最多购买2次";
        public int DiamondsNeed { get; set; } = 50;
        public bool Purchase(MsgInformationEx MsgDTO)
        {
            var startTime = DateTime.Now.Date;
            var endTime = startTime.AddDays(1);
            var purchaseRec = MongoService<VipSvcPurchaseRecord>.Get(p =>
                p.QQNum == MsgDTO.FromQQ && p.SvcName == Name && p.PurchaseTime > startTime && p.PurchaseTime <= endTime);
            if (purchaseRec.Count >= 2)
            {
                MsgSender.PushMsg(MsgDTO, "你今天已经买了2次了", true);
                return false;
            }

            var shopItems = TransHelper.GetDailySellItems();
            var randItem = shopItems.RandElement();
            var itemColle = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var msg = itemColle.ItemIncome(randItem.Name);
            msg = $"恭喜你获得了 {randItem.Name}\r{msg}";
            MsgSender.PushMsg(MsgDTO, msg);

            return true;
        }
    }
}
