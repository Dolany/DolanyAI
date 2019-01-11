namespace Dolany.Ai.Core.AITools
{
    using System.Timers;

    using Cache;
    using Common;

    public class PicCacheTool : IAITool
    {
        public void Work()
        {
            PicCacher.Load();

            JobScheduler.Instance.Add(10 * JobTimer.MinutelyInterval, TimeUp);
        }

        private static void TimeUp(object sender, ElapsedEventArgs e)
        {
            PicCacher.Save();
        }
    }
}
