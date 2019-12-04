using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.API;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.OnlineStore;

namespace Dolany.Ai.Core.Ai.Vip
{
    public class DailyVipShopMgr
    {
        public static DailyVipShopMgr Instance { get; } = new DailyVipShopMgr();

        private delegate bool ServiceDel(MsgInformationEx MsgDTO);

        private readonly Dictionary<VipServiceAttribute, ServiceDel> ServiceDic = new Dictionary<VipServiceAttribute, ServiceDel>();

        public VipServiceAttribute this[string name] => ServiceDic.Keys.FirstOrDefault(s => s.Name == name);

        private DailyVipShopMgr()
        {
            var t = GetType();
            foreach (var method in t.GetMethods())
            {
                foreach (VipServiceAttribute attr in method.GetCustomAttributes(typeof(VipServiceAttribute), false))
                {
                    var attrClone = attr.Clone();
                    ServiceDic.Add(attrClone, method.CreateDelegate(typeof(ServiceDel), this) as ServiceDel);
                }
            }
        }

        public string[] RandGoods(int count)
        {
            return Rander.RandSort(ServiceDic.Keys.ToArray()).Take(count).Select(p => p.Name).ToArray();
        }

        public void Serve(MsgInformationEx MsgDTO, string serviceName)
        {
            var attr = ServiceDic.Keys.FirstOrDefault(p => p.Name == serviceName);
            if (attr == null)
            {
                MsgSender.PushMsg(MsgDTO, $"未找到相关内容：{serviceName}");
                return;
            }

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.Diamonds < attr.DiamondsNeed)
            {
                MsgSender.PushMsg(MsgDTO, $"钻石余额不足({osPerson.Diamonds}/{attr.DiamondsNeed})！");
                return;
            }

            if (!Waiter.Instance.WaitForConfirm(MsgDTO, $"此操作将花费{attr.DiamondsNeed}{Emoji.钻石}，是否继续？"))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return;
            }

            if (!ServiceDic[attr](MsgDTO))
            {
                return;
            }

            osPerson.Diamonds -= attr.DiamondsNeed;
            osPerson.Update();

            var purchaseRec = new VipSvcPurchaseRecord()
            {
                QQNum = MsgDTO.FromQQ,
                SvcName = attr.Name,
                PurchaseTime = DateTime.Now,
                Diamonds = attr.DiamondsNeed
            };
            purchaseRec.Insert();
        }

        [VipService(Name = "耐力护符", Description = "使宠物的耐力上限增加10，持续10天(同时只能持有一个)", DiamondsNeed = 20)]
        public bool ExpandPetEndurance(MsgInformationEx MsgDTO)
        {
            var armerRec = VipArmer.Get(MsgDTO.FromQQ);
            if (armerRec.CheckArmer("耐力护符"))
            {
                MsgSender.PushMsg(MsgDTO, "你已经持有一个耐力护符了！");
                return false;
            }

            var armer = new ArmerModel() {Name = "耐力护符", Description = "使宠物的耐力上限增加10，持续10天。", ExpiryTime = DateTime.Now.AddDays(10)};
            armerRec.Armers.Add(armer);
            armerRec.Update();

            MsgSender.PushMsg(MsgDTO, $"购买成功！有效期至：{armer.ExpiryTime:yyyy-MM-dd HH:mm:ss}");
            return true;
        }
    }
}
