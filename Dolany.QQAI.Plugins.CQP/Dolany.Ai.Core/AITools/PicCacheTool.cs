using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.AITools
{
    using System.Timers;

    using Dolany.Ai.Core.Cache;
    using Dolany.Ai.Core.Common;

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
