using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        private List<IVipArmer> Armers { get; set; }

        public IVipArmer this[string Name] => Armers.FirstOrDefault(p => p.Name == Name);

        private DailyVipShopMgr()
        {
            var assembly = Assembly.GetAssembly(typeof(IVipArmer));
            Armers = assembly.GetTypes()
                .Where(type => typeof(IVipArmer).IsAssignableFrom(type) && type.IsClass)
                .Where(type => type.FullName != null)
                .Select(type => new { type, checker = assembly.CreateInstance(type.FullName) as IVipArmer })
                .Select(t => t.checker).ToList();
        }

        public string[] RandGoods(int count)
        {
            return Rander.RandSort(Armers.ToArray()).Take(count).Select(p => p.Name).ToArray();
        }

        public void Serve(MsgInformationEx MsgDTO, string serviceName)
        {
            var armer = Armers.FirstOrDefault(p => p.Name == serviceName);
            if (armer == null)
            {
                MsgSender.PushMsg(MsgDTO, $"未找到相关内容：{serviceName}");
                return;
            }

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.Diamonds < armer.DiamondsNeed)
            {
                MsgSender.PushMsg(MsgDTO, $"钻石余额不足({osPerson.Diamonds}/{armer.DiamondsNeed})！");
                return;
            }

            if (!Waiter.Instance.WaitForConfirm(MsgDTO, $"此操作将花费{armer.DiamondsNeed}{Emoji.钻石}，是否继续？"))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return;
            }

            if (!armer.Purchase(MsgDTO))
            {
                return;
            }

            osPerson.Diamonds -= armer.DiamondsNeed;
            osPerson.Update();

            var purchaseRec = new VipSvcPurchaseRecord()
            {
                QQNum = MsgDTO.FromQQ,
                SvcName = armer.Name,
                PurchaseTime = DateTime.Now,
                Diamonds = armer.DiamondsNeed
            };
            purchaseRec.Insert();
        }
    }
}
