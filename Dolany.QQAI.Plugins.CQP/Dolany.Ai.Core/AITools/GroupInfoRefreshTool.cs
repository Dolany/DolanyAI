﻿using Dolany.Ai.Common;
using Dolany.Ai.Core.Common;
using System.Collections.Generic;

namespace Dolany.Ai.Core.AITools
{
    public class GroupInfoRefreshTool : IScheduleTool
    {
        protected override List<ScheduleDoModel> ModelList { get; set; } = new List<ScheduleDoModel>()
        {
            new ScheduleDoModel()
            {
                Interval = SchedulerTimer.HourlyInterval
            }
        };

        public override bool Enabled { get; set; } = true;

        protected override void ScheduleDo(SchedulerTimer timer)
        {
            GroupSettingMgr.Instance.Refresh();
        }
    }
}
