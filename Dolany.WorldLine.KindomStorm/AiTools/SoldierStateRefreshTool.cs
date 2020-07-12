using System;
using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;

namespace Dolany.WorldLine.KindomStorm.AiTools
{
    public class SoldierStateRefreshTool : IScheduleTool
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
            if (!CheckTodayRefresh())
            {
                return;
            }


        }

        private bool CheckTodayRefresh()
        {
            var rec = GlobalVarRecord.Get("SoldierRefresh");
            if (string.IsNullOrEmpty(rec.Value) || !DateTime.TryParse(rec.Value, out var lastTime) || lastTime >= DateTime.Today)
            {
                return false;
            }

            rec.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            rec.Update();
            return true;
        }
    }
}
