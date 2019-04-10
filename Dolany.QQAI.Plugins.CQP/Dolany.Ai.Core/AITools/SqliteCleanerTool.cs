using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Database.Sqlite;

namespace Dolany.Ai.Core.AITools
{
    public class SqliteCleanerTool : IScheduleTool
    {
        protected override List<ScheduleDoModel> ModelList { get; set; } = new List<ScheduleDoModel>()
        {
            new ScheduleDoModel()
            {
                Interval = SchedulerTimer.DairlyInterval
            }
        };
        protected override void ScheduleDo(SchedulerTimer timer)
        {
            SCacheService.CheckOutOfDate();
        }
    }
}
