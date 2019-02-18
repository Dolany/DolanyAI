using System.Collections.Generic;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.AITools
{
    using Cache;

    public class PicCacheTool : IScheduleTool
    {
        protected override List<ScheduleDoModel> ModelList { get; set; } =
            new List<ScheduleDoModel>() {new ScheduleDoModel() {Interval = 10 * SchedulerTimer.MinutelyInterval}};

        public override void Work()
        {
            PicCacher.Load();
            base.Work();
        }

        protected override void ScheduleDo(SchedulerTimer timer)
        {
            PicCacher.Save();
        }
    }
}
