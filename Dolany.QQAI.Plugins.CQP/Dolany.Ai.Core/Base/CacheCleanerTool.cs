namespace Dolany.Ai.Core.AITools
{
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Timers;

    using Common;

    using Dolany.Ai.Common;

    using static API.CodeApi;

    public class CacheCleanerDTO
    {
        public string Path { get; set; }

        public bool IsCascading { get; set; }

        public int MaxCacheCount { get; set; }
    }

    public class CacheCleanerTool : IAITool
    {
        private readonly int PicCleanFreq = int.Parse(Configger.Instance["PicCleanFreq"]);

        private static readonly int MaxPicCache = int.Parse(Configger.Instance["MaxOriginPicCache"]);

        public void Work()
        {
            RuntimeLogger.Log($"{nameof(CacheCleanerTool)} started.");
            JobScheduler.Instance.Add(
                PicCleanFreq * 1000,
                TimeUp,
                new CacheCleanerDTO
                {
                    Path = ImagePath,
                    IsCascading = false,
                    MaxCacheCount = MaxPicCache
                    });
            JobScheduler.Instance.Add(
                JobTimer.DairlyInterval,
                TimeUp,
                new CacheCleanerDTO
                {
                    Path = "c:/AmandaQQ/logs/",
                    IsCascading = false,
                    MaxCacheCount = 7
                    });
            JobScheduler.Instance.Add(
                JobTimer.DairlyInterval,
                TimeUp,
                new CacheCleanerDTO
                {
                    Path = "RuntimeLog/",
                    IsCascading = false,
                    MaxCacheCount = 7
                    });
            JobScheduler.Instance.Add(
                JobTimer.HourlyInterval / 2,
                TimeUp,
                new CacheCleanerDTO
                {
                    Path = "c:/AmandaQQ/temp/voice/",
                    IsCascading = false,
                    MaxCacheCount = 50
                    });
            JobScheduler.Instance.Add(
                JobTimer.HourlyInterval / 2,
                TimeUp,
                new CacheCleanerDTO
                {
                    Path = "c:/AmandaQQ/VoiceCache/",
                    IsCascading = false,
                    MaxCacheCount = 50
                    });
        }

        private void TimeUp(object sender, ElapsedEventArgs e)
        {
            var timer = sender as JobTimer;
            Debug.Assert(timer != null, nameof(timer) + " != null");
            var dto = timer.Data as CacheCleanerDTO;

            CleanCache(dto);
        }

        private static void CleanCache(CacheCleanerDTO dto)
        {
            var dir = new DirectoryInfo(dto.Path);
            var cleanCount = dir.GetFiles().Length - dto.MaxCacheCount;
            if (cleanCount <= 0)
            {
                return;
            }

            var cleanFiles = dir.GetFiles().OrderBy(f => f.CreationTime)
                                           .Take(cleanCount);
            foreach (var f in cleanFiles)
            {
                f.Delete();
            }

            if (!dto.IsCascading)
            {
                return;
            }

            foreach (var subDir in dir.GetDirectories())
            {
                dto.Path = subDir.FullName;
                CleanCache(dto);
            }
        }
    }
}
