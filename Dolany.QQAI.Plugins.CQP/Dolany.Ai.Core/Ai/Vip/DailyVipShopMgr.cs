using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Vip
{
    public class DailyVipShopMgr
    {
        public static DailyVipShopMgr Instance { get; } = new DailyVipShopMgr();

        private List<IVipArmer> Armers { get; set; }

        public IVipArmer this[string Name] => Armers.FirstOrDefault(p => p.Name == Name);

        private DailyVipShopMgr()
        {
            Armers = CommonUtil.LoadAllInstanceFromInterface<IVipArmer>();
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
                MsgSender.PushMsg(MsgDTO, $"钻石余额不足({osPerson.Diamonds.CurencyFormat("Diamond")}/{armer.DiamondsNeed.CurencyFormat("Diamond")})！");
                return;
            }

            if (!CheckLimit(MsgDTO, armer))
            {
                return;
            }

            if (!CheckMaxContains(MsgDTO, armer))
            {
                return;
            }

            if (!Waiter.Instance.WaitForConfirm(MsgDTO, $"此操作将花费{armer.DiamondsNeed.CurencyFormat("Diamond")}，是否继续？"))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return;
            }

            if (!armer.Purchase(MsgDTO))
            {
                return;
            }

            osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
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

        private static bool CheckMaxContains(MsgInformationEx MsgDTO, IVipArmer armer)
        {
            if (armer.MaxContains == 0)
            {
                return true;
            }

            var armerRec = VipArmerRecord.Get(MsgDTO.FromQQ);
            if (!armerRec.CheckArmer(armer.Name, armer.MaxContains))
            {
                return true;
            }

            MsgSender.PushMsg(MsgDTO, $"你已经有{armer.MaxContains}件这个装备了！");
            return false;
        }

        private static bool CheckLimit(MsgInformationEx MsgDTO, IVipArmer armer)
        {
            if (armer.LimitCount == 0)
            {
                return true;
            }

            var (startDate, endDate) = ParseDateRange(armer.LimitInterval);
            var purchaseRec = MongoService<VipSvcPurchaseRecord>.Get(p =>
                p.QQNum == MsgDTO.FromQQ && p.SvcName == armer.Name && p.PurchaseTime > startDate && p.PurchaseTime <= endDate);
            if (purchaseRec.Count < armer.LimitCount)
            {
                return true;
            }

            MsgSender.PushMsg(MsgDTO, $"你{LimitIntervalToString(armer.LimitInterval)}已经买了{armer.LimitCount}次了", true);
            return false;
        }

        private static string LimitIntervalToString(VipArmerLimitInterval interval)
        {
            return interval switch
            {
                VipArmerLimitInterval.Daily => "今天",
                VipArmerLimitInterval.Weekly => "本周",
                VipArmerLimitInterval.Monthly => "本月",
                _ => default
            };
        }

        private static (DateTime, DateTime) ParseDateRange(VipArmerLimitInterval interval)
        {
            switch (interval)
            {
                case VipArmerLimitInterval.Daily:
                {
                    return (DateTime.Today, DateTime.Today.AddDays(1));
                }
                case VipArmerLimitInterval.Weekly:
                {
                    var startDate = DateTime.Now.DayOfWeek == DayOfWeek.Sunday ? DateTime.Today.AddDays(-6) : DateTime.Today.AddDays(1 - (int)DateTime.Now.DayOfWeek);
                    var endDate = startDate.AddDays(7);
                    return (startDate, endDate);
                }
                case VipArmerLimitInterval.Monthly:
                {
                    var startDate = DateTime.Today.AddDays(1 - DateTime.Now.Day);
                    var endDate = startDate.AddMonths(1);
                    return (startDate, endDate);
                }
                default:
                {
                    return default;
                }
            }
        }
    }
}
