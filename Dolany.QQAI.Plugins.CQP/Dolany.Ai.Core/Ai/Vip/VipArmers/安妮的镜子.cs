using System;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Vip.VipArmers
{
    public class 安妮的镜子 : IVipArmer
    {
        public string Name { get; set; } = "安妮的镜子";
        public string Description { get; set; } = "额外获得一个下次捞瓶子获得的物品的复制，最多同时持有1个，每天最多购买2次。";
        public int DiamondsNeed { get; set; } = 30;
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

            var armerRec = VipArmerRecord.Get(MsgDTO.FromQQ);
            if (armerRec.CheckArmer(Name))
            {
                MsgSender.PushMsg(MsgDTO, "你已经有一件这个装备了！");
                return false;
            }

            var armer = new ArmerModel {Name = Name, Description = Description};
            armerRec.Armers.Add(armer);
            armerRec.Update();

            MsgSender.PushMsg(MsgDTO, "购买成功！");
            return true;
        }
    }
}
