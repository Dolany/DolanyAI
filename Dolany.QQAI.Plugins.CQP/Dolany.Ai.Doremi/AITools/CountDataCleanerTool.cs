using System;
using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Doremi.Ai.Game.Xiuxian;

namespace Dolany.Ai.Doremi.AITools
{
    public class CountDataCleanerTool : IScheduleTool
    {
        protected override List<ScheduleDoModel> ModelList { get; set; } = new List<ScheduleDoModel>()
        {
            new ScheduleDoModel()
            {
                Interval = SchedulerTimer.HourlyInterval
            }
        };

        protected override void ScheduleDo(SchedulerTimer timer)
        {
            if (DateTime.Now.DayOfWeek != DayOfWeek.Monday)
            {
                return;
            }

            var record = GlobalVarRecord.Get("LastCountCleanDate");
            var todayStr = DateTime.Now.ToString("yyyyMMdd");
            if (record.Value == todayStr)
            {
                return;
            }

            MsgCounterSvc.CleanAll();
            record.Value = todayStr;
            record.Update();
        }
    }
}
