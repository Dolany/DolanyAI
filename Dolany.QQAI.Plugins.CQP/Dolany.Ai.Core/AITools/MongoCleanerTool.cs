using Dolany.Ai.Common;
using Dolany.Database;
using Dolany.Database.Ai;
using System.Collections.Generic;

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

        public override bool Enabled { get; set; } = true;

        protected override void ScheduleDo(SchedulerTimer timer)
        {
            var outOfDateRecords = MongoService<DriftBottleRecord>.Get(r => r.ReceivedQQ != null);
            MongoService<DriftBottleRecord>.DeleteMany(outOfDateRecords);
        }
    }
}
