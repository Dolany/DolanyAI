using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Common;

namespace Dolany.Ai.Core.AITools
{
    public class CommandRecorderTool : IScheduleTool
    {
        private int cache;

        protected override List<ScheduleDoModel> ModelList { get; set; } = new List<ScheduleDoModel>(){new ScheduleDoModel()
        {
            Interval = SchedulerTimer.HourlyInterval
        }};
        protected override void ScheduleDo(SchedulerTimer timer)
        {
            var count = AIAnalyzer.GetCommandCount();
            AIAnalyzer.CountRecord(count - cache);
            cache = count;
        }
    }
}
