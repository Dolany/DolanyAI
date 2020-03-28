using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;

namespace Dolany.WorldLine.Standard.AITools
{
    public class RefreshRecentCmdTool : IScheduleTool
    {
        public RestrictorSvc RestrictorSvc { get; set; }

        protected override List<ScheduleDoModel> ModelList { get; set; } = new List<ScheduleDoModel>()
        {
            new ScheduleDoModel()
            {
                Interval = SchedulerTimer.MinutelyInterval
            }
        };
        protected override void ScheduleDo(SchedulerTimer timer)
        {
            RestrictorSvc.CleanOutOfDateData();
        }
    }
}
