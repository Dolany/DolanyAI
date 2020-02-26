using System.Collections.Generic;
using System.Timers;
using Dolany.Ai.Common;

namespace Dolany.Ai.Doremi.AITools
{
    public abstract class IScheduleTool : IAITool
    {
        protected abstract List<ScheduleDoModel> ModelList { get; set; }
        public abstract bool Enable { get; set; }

        protected static Scheduler Scheduler => AutofacSvc.Resolve<Scheduler>();

        public virtual void Work()
        {
            foreach (var model in ModelList)
            {
                Scheduler.Add(model.Interval, TimeUp, model.Data, IsImmdiately:model.IsImmediately);
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

        public bool IsImmediately { get; set; } = false;

        public object Data { get; set; }
    }
}
