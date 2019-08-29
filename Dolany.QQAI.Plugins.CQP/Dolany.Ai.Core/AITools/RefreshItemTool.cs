using Dolany.Ai.Common;
using Dolany.Ai.Core.OnlineStore;
using System;
using System.Collections.Generic;

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

        public override bool Enabled { get; set; } = true;

        protected override void ScheduleDo(SchedulerTimer timer)
        {
            HonorHelper.Instance.Refresh();

            timer.Interval = (DateTime.Now.Date.AddDays(1) - DateTime.Now).TotalMilliseconds;
        }
    }
}
