using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Newtonsoft.Json;

namespace Dolany.WorldLine.Doremi.Xiuxian
{
    public class RandShopperSvc : IDependency
    {
        private List<ShoppingNoticeModel> Models = new List<ShoppingNoticeModel>();

        public ArmerSvc ArmerSvc { get; set; }
        public GroupSettingSvc GroupSettingSvc { get; set; }

        public string[] SellingGoods;

        public string BindAi { get; set; }

        public SchedulerSvc SchedulerSvc { get; set; }

        public void Refresh()
        {
            foreach (var model in Models.Where(model => !string.IsNullOrEmpty(model.TimerID)))
            {
                SchedulerSvc.Remove(model.TimerID);
            }

            Models = new List<ShoppingNoticeModel>();

            var shopInfo = GlobalVarRecord.Get("TodayShopInfo");
            if (string.IsNullOrEmpty(shopInfo.Value))
            {
                return;
            }

            var records = JsonConvert.DeserializeObject<List<ShopTimeRecord>>(shopInfo.Value);
            foreach (var record in records)
            {
                Models.Add(new ShoppingNoticeModel()
                {
                    NoticeTime = record.OpenTime,
                    Type = ShoppingNoticeType.OpenShop,
                    Data = record.SellingGoods
                });

                Models.Add(new ShoppingNoticeModel()
                {
                    NoticeTime = record.OpenTime.AddMinutes(-30),
                    Type = ShoppingNoticeType.Forecast
                });

                Models.Add(new ShoppingNoticeModel()
                {
                    NoticeTime = record.OpenTime.AddMinutes(5),
                    Type = ShoppingNoticeType.CloseShop
                });
            }

            Models = Models.Where(p => p.NoticeTime > DateTime.Now).OrderBy(p => p.NoticeTime).ToList();
            foreach (var model in Models)
            {
                var interval = (model.NoticeTime - DateTime.Now).TotalMilliseconds;
                model.TimerID = SchedulerSvc.Add(interval, TimeUp, model, false);
            }
        }

        private void TimeUp(object sender, ElapsedEventArgs e)
        {
            var timer = sender as SchedulerTimer;

            if (!(timer?.Data is ShoppingNoticeModel model))
            {
                return;
            }

            switch (model.Type)
            {
                case ShoppingNoticeType.Forecast:
                    Broadcast("商店将于半个小时后开启！");
                    break;
                case ShoppingNoticeType.OpenShop:
                    SellingGoods = (string[]) model.Data;
                    var goodsMsg = string.Join("\r\n", SellingGoods.Select(goods => $"{goods}:{ArmerSvc[goods].Price.CurencyFormat()}"));
                    Broadcast($"系统商店开启！当前售卖的商品有：\r\n{goodsMsg}\r\n请使用 购买 [商品名] 命令来购买指定商品！");
                    break;
                case ShoppingNoticeType.CloseShop:
                    Broadcast("系统商店已关闭！");
                    SellingGoods = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Broadcast(string msg)
        {
            foreach (var group in GroupSettingSvc.SettingDic.Values.Where(p => p.IsPowerOn && p.WorldLine == "Doremi"))
            {
                MsgSender.PushMsg(group.GroupNum, 0, msg, BindAi);
            }
        }
    }

    public class ShoppingNoticeModel
    {
        public DateTime NoticeTime { get; set; }

        public ShoppingNoticeType Type { get; set; }

        public object Data { get; set; }

        public string TimerID { get; set; }
    }

    public enum ShoppingNoticeType
    {
        Forecast,
        OpenShop,
        CloseShop
    }
}
