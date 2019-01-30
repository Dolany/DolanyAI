using Dolany.Ai.Common;

namespace Dolany.Ai.Core.AITools
{
    using System.Timers;

    using Cache;

    public class PicCacheTool : IAITool
    {
        public void Work()
        {
            PicCacher.Load();

            Scheduler.Instance.Add(10 * SchedulerTimer.MinutelyInterval, TimeUp);
        }

        private static void TimeUp(object sender, ElapsedEventArgs e)
        {
            PicCacher.Save();
        }
    }
}
