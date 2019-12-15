using System;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Ai.Vip.VipArmers
{
    public class 黑金之匣 : IVipArmer
    {
        public string Name { get; set; } = "黑金之匣";
        public string Description { get; set; } = "随机获取一个稀有商店售卖的商品(仅在稀有商店开启时可用)，每天最多购买2次";
        public int DiamondsNeed { get; set; } = 150;
        public VipArmerLimitInterval LimitInterval { get; set; } = VipArmerLimitInterval.Daily;
        public int LimitCount { get; set; } = 2;
        public bool Purchase(MsgInformationEx MsgDTO)
        {
            var todayRec = DailySellItemRareRecord.GetToday();
            if (!todayRec.IsActive)
            {
                MsgSender.PushMsg(MsgDTO, "稀有商店尚未开启，你无法购买此物品！");
                return false;
            }

            var randItem = todayRec.Items.RandElement();
            var itemColle = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var msg = itemColle.ItemIncome(randItem.Name);

            MsgSender.PushMsg(MsgDTO, $"你获得了{randItem.Name}\r{msg}");
            return true;
        }

        public int MaxContains { get; set; }
    }
}
