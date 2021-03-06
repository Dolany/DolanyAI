﻿using Dolany.Ai.Common;
using System;
using System.Collections.Generic;
using Dolany.Ai.Core.Base;
using Dolany.WorldLine.Standard.OnlineStore;

namespace Dolany.WorldLine.Standard.AITools
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
            AutofacSvc.Resolve<HonorSvc>().RefreshData();

            timer.Interval = (DateTime.Now.Date.AddDays(1) - DateTime.Now).TotalMilliseconds;
        }
    }
}
