using System;
using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Core.OnlineStore;

namespace Dolany.Ai.Core.AITools
{
    public class RefreshItemTool : IScheduleTool
    {
        protected override List<ScheduleDoModel> ModelList { get; set; } = new List<ScheduleDoModel>()
        {
            new ScheduleDoModel()
            {
                Interval = (DateTime.Now.Date.AddDays(1) - DateTime.Now).TotalMilliseconds
            }
        };
        protected override void ScheduleDo(SchedulerTimer timer)
        {
            HonorHelper.Instance.Refresh();

            timer.Interval = (DateTime.Now.Date.AddDays(1) - DateTime.Now).TotalMilliseconds;
        }
    }
}
