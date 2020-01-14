using System.Linq;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;
using Dolany.Database.Ai;
using Dolany.WorldLine.Standard.OnlineStore;

namespace Dolany.WorldLine.Standard.Ai.Vip.VipArmers
{
    public class 星耀之匣 : IVipArmer
    {
        public string Name { get; set; } = "星耀之匣";
        public string Description { get; set; } = "获取稀有商店中的所有物品，每周限购一次。";
        public int DiamondsNeed { get; set; } = 800;
        public VipArmerLimitInterval LimitInterval { get; set; } = VipArmerLimitInterval.Weekly;
        public int LimitCount { get; set; } = 1;
        public bool Purchase(MsgInformationEx MsgDTO)
        {
            var todayRec = DailySellItemRareRecord.GetToday();
            if (!todayRec.IsActive)
            {
                MsgSender.PushMsg(MsgDTO, "稀有商店尚未开启，你无法购买此物品！");
                return false;
            }

            var itemColle = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var msgs = todayRec.Items.Select(item => itemColle.ItemIncome(item.Name));
            MsgSender.PushMsg(MsgDTO, string.Join("\r", msgs));
            return true;
        }

        public int MaxContains { get; set; }
    }
}
