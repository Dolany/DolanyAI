using System;

namespace Dolany.Cache
{
    using System.Timers;

    using Dolany.Ai.Common;

    class Program
    {
        private static CacheService cacheService;

        static void Main(string[] args)
        {
            cacheService = new CacheService("CacheInfoService");
            Scheduler.Instance.Add(10 * SchedulerTimer.MinutelyInterval, TimeUp);

            var command = Console.ReadLine();
            while (command != "Exit")
            {
                command = Console.ReadLine();
            }
        }

        private static void TimeUp(object sender, ElapsedEventArgs e)
        {
            cacheService.Refresh();
        }
    }
}
