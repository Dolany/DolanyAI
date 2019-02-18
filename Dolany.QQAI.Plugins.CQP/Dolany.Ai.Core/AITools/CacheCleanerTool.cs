using System.Collections.Generic;

namespace Dolany.Ai.Core.AITools
{
    using System.IO;
    using System.Linq;
    using Dolany.Ai.Common;

    using static API.CodeApi;

    public class CacheCleanerDTO
    {
        public string Path { get; set; }

        public bool IsCascading { get; set; }

        public int MaxCacheCount { get; set; }
    }

    public class CacheCleanerTool : IScheduleTool
    {
        private readonly int PicCleanFreq = int.Parse(Configger.Instance["PicCleanFreq"]);

        private readonly int MaxPicCache = int.Parse(Configger.Instance["MaxOriginPicCache"]);

        protected override List<ScheduleDoModel> ModelList { get; set; } = new List<ScheduleDoModel>();

        private void InitModelList()
        {
            ModelList.Add(new ScheduleDoModel()
            {
                Interval = PicCleanFreq * 1000,
                Data = new CacheCleanerDTO
                {
                    Path = ImagePath,
                    IsCascading = false,
                    MaxCacheCount = MaxPicCache
                }
            });
            ModelList.Add(new ScheduleDoModel()
            {
                Interval = SchedulerTimer.DairlyInterval,
                Data = new CacheCleanerDTO
                {
                    Path = "c:/AmandaQQ/logs/",
                    IsCascading = false,
                    MaxCacheCount = 7
                }
            });
            ModelList.Add(new ScheduleDoModel()
            {
                Interval = SchedulerTimer.DairlyInterval,
                Data = new CacheCleanerDTO
                {
                    Path = "RuntimeLog/",
                    IsCascading = false,
                    MaxCacheCount = 7
                }
            });
            ModelList.Add(new ScheduleDoModel()
            {
                Interval = SchedulerTimer.HourlyInterval / 2,
                Data = new CacheCleanerDTO
                {
                    Path = "c:/AmandaQQ/temp/voice/",
                    IsCascading = false,
                    MaxCacheCount = 50
                }
            });
        }

        public override void Work()
        {
            InitModelList();
            base.Work();
        }

        protected override void ScheduleDo(SchedulerTimer timer)
        {
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
