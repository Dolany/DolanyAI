using System.Collections.Generic;
using System.Timers;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.AITools
{
    public abstract class IScheduleTool : IAITool
    {
        protected abstract List<ScheduleDoModel> ModelList { get; set; }

        public virtual void Work()
        {
            foreach (var model in ModelList)
            {
                Scheduler.Instance.Add(model.Interval, TimeUp, model.Data);
            }
        }

        private void TimeUp(object sender, ElapsedEventArgs e)
        {
            var timer = sender as SchedulerTimer;
            ScheduleDo(timer);
        }

        protected abstract void ScheduleDo(SchedulerTimer timer);
    }

    public class ScheduleDoModel
    {
        public double Interval { get; set; }

        public object Data { get; set; }
    }
}
