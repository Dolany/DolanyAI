using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Doremi.Cache;
using Dolany.Ai.Doremi.Xiuxian;
using Dolany.Database;
using Newtonsoft.Json;

namespace Dolany.Ai.Doremi.AITools
{
    public class ShopTool : IScheduleTool
    {
        public override bool Enable { get; set; } = true;

        protected override List<ScheduleDoModel> ModelList { get; set; } = new List<ScheduleDoModel>()
        {
            new ScheduleDoModel()
            {
                Interval = SchedulerTimer.HourlyInterval,
                IsImmediately = true
            }
        };

        protected override void ScheduleDo(SchedulerTimer timer)
        {
            var todayRecord = MongoService<GlobalVarRecord>.GetOnly(p => p.Key == "TodayShopInfo");
            if (!string.IsNullOrEmpty(todayRecord.Value))
            {
                return;
            }

            var records = GetHours(3).Select(hour =>
            {
                var isHalfHour = CommonUtil.RandInt(2) == 0;
                var dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, isHalfHour ? 30 : 0, 0);
                var goods = ArmerMgr.Instance.GetRandArmers(5).Select(p => p.Name);
                return new ShopTimeRecord() {OpenTime = dateTime, SellingGoods = goods.ToArray()};
            }).ToList();

            todayRecord.Value = JsonConvert.SerializeObject(records);
            todayRecord.ExpiryTime = CommonUtil.UntilTommorow();
            todayRecord.Update();

            RandShopper.Instance.Refresh();
        }

        private static IEnumerable<int> GetHours(int count)
        {
            var validHours = new List<int>();
            for (var i = 2; i <= 23; i++)
            {
                validHours.Add(i);
            }

            var result = new int[count];
            for (var i = 0; i < count; i++)
            {
                result[i] = validHours.RandElement();
                validHours.RemoveAll(p => p >= result[i] - 1 && p <= result[i] + 1);
            }

            return result;
        }
    }
}
