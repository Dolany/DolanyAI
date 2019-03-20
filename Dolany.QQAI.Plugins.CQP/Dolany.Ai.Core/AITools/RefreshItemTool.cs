using System;
using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Game.OnlineStore;

namespace Dolany.Ai.Core.AITools
{
    public class RefreshItemTool : IScheduleTool
    {
        protected override List<ScheduleDoModel> ModelList { get; set; } = new List<ScheduleDoModel>()
        {
            new ScheduleDoModel()
            {
                Interval = DateTime.Now.Hour < 4 ? (DateTime.Now.Date.AddHours(4) - DateTime.Now).TotalMilliseconds :
                    (DateTime.Now.AddDays(1).Date.AddHours(4) - DateTime.Now).TotalMilliseconds
            }
        };
        protected override void ScheduleDo(SchedulerTimer timer)
        {
            HonorHelper.Instance.Refresh();

            timer.Interval = DateTime.Now.Hour < 4
                ? (DateTime.Now.Date.AddHours(4) - DateTime.Now).TotalMilliseconds
                : (DateTime.Now.AddDays(1).Date.AddHours(4) - DateTime.Now).TotalMilliseconds;
        }
    }
}
