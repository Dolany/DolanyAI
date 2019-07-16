using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Doremi.Cache;
using Dolany.Ai.Doremi.Xiuxian;
using Dolany.Database;

namespace Dolany.Ai.Doremi.AITools
{
    public class ShopTool : IScheduleTool
    {
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
            if (todayRecord != null)
            {
                return;
            }

            // todo

            RandShopper.Instance.Refresh();
        }
    }
}
