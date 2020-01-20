using Dolany.Ai.Common;
using System.Collections.Generic;
using System.Timers;

namespace Dolany.Ai.Core.Base
{
    public abstract class IScheduleTool : IAITool
    {
        protected abstract List<ScheduleDoModel> ModelList { get; set; }

        public virtual bool Enabled { get; set; } = true;

        public virtual void Work()
        {
            foreach (var model in ModelList)
            {
                Scheduler.Instance.Add(model.Interval, TimeUp, model.Data, IsImmdiately:model.IsImmediately);
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

        public bool IsImmediately { get; set; }

        public object Data { get; set; }
    }
}
