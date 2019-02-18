using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.AITools
{
    public class MongoCleanerTool : IScheduleTool
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
            var outOfDateRecords = MongoService<DriftBottleRecord>.Get(r => r.ReceivedQQ != null);
            MongoService<DriftBottleRecord>.DeleteMany(outOfDateRecords);
        }
    }
}
