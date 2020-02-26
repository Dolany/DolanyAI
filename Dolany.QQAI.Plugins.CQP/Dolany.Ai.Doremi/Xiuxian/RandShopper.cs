using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Dolany.Ai.Common;
using Dolany.Ai.Doremi.Cache;
using Dolany.Ai.Doremi.Common;
using Newtonsoft.Json;

namespace Dolany.Ai.Doremi.Xiuxian
{
    public class RandShopper
    {
        private List<ShoppingNoticeModel> Models = new List<ShoppingNoticeModel>();

        public string[] SellingGoods;

        public string BindAi { private get; set; }

        public Scheduler Scheduler { get; set; }

        private RandShopper()
        {
            Refresh();
        }

        public void Refresh()
        {
            foreach (var model in Models.Where(model => !string.IsNullOrEmpty(model.TimerID)))
            {
                Scheduler.Remove(model.TimerID);
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
                //model.TimerID = Scheduler.Instance.Add(interval, TimeUp, model, false);
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
                    SellingGoods = model.Data as string[];
                    var goodsMsg = string.Join("\r", SellingGoods?.Select(goods => $"{goods}:{ArmerMgr.Instance[goods].Price}金币"));
                    Broadcast($"系统商店开启！当前售卖的商品有：\r{goodsMsg}\r请使用 购买 [商品名] 命令来购买指定商品！");
                    break;
                case ShoppingNoticeType.CloseShop:
                    Broadcast("系统商店已关闭！");
                    SellingGoods = null;
                    break;
            }
        }

        private void Broadcast(string msg)
        {
            if (!PowerStateMgr.Instance.CheckPower(BindAi))
            {
                return;
            }

            foreach (var (groupNum, _) in GroupSettingMgr.Instance.SettingDic)
            {
                MsgSender.PushMsg(groupNum, 0, msg, BindAi);
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
