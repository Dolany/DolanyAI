using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.AITools
{
    public class RareShopUpdateTool : IScheduleTool
    {
        protected override List<ScheduleDoModel> ModelList { get; set; } = new List<ScheduleDoModel>()
        {
            new ScheduleDoModel()
            {
                Interval = SchedulerTimer.HourlyInterval,
                IsImmediately = true
            }
        };
        public override bool Enabled { get; set; } = true;
        protected override void ScheduleDo(SchedulerTimer timer)
        {
            var todayRec = DailySellItemRareRecord.GetToday();
            if (todayRec.Items.IsNullOrEmpty())
            {
                todayRec.Items = CreateDailySellItems_Rare();
                todayRec.Hour = Rander.RandInt(22);

                todayRec.Update();
            }

            var tomorrowRec = DailySellItemRareRecord.GetTomorrow();
            if (tomorrowRec.Items.IsNullOrEmpty())
            {
                tomorrowRec.Items = CreateDailySellItems_Rare();
                tomorrowRec.Hour = Rander.RandInt(22);

                tomorrowRec.Update();
            }
        }

        private static DailySellItemModel[] CreateDailySellItems_Rare()
        {
            var honors = HonorHelper.Instance.HonorList.Where(h => !h.IsLimit);
            var items = honors.SelectMany(h => h.Items).Where(p => p.Price >= 500).ToArray();
            var randSort = Rander.RandSort(items).Take(5);
            return randSort.Select(rs => new DailySellItemModel
            {
                Name = rs.Name,
                Price = rs.Price * 2,
                Attr = string.Join(",", rs.Attributes)
            }).ToArray();
        }
    }
}
