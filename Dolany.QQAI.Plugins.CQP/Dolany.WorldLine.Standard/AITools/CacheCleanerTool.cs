using Dolany.Ai.Common;
using Dolany.Ai.Core.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dolany.Ai.Core.Base;

namespace Dolany.WorldLine.Standard.AITools
{
    public class CacheCleanerDTO
    {
        public string Path { get; set; }

        public bool IsCascading { get; set; }

        public int MaxCacheCount { get; set; }
    }

    public class CacheCleanerTool : IScheduleTool
    {
        private const int PicCleanFreq = 10;

        private const int MaxPicCache = 20;

        protected override List<ScheduleDoModel> ModelList { get; set; } = new List<ScheduleDoModel>();
        public override bool Enabled { get; set; } = true;

        public BindAiSvc BindAiSvc { get; set; }

        private void InitModelList()
        {
            foreach (var (_, value) in BindAiSvc.AiDic)
            {
                ModelList.Add(new ScheduleDoModel()
                {
                    Interval = PicCleanFreq * 1000,
                    Data = new CacheCleanerDTO
                    {
                        Path = value.ImagePath,
                        IsCascading = false,
                        MaxCacheCount = MaxPicCache
                    }
                });
                ModelList.Add(new ScheduleDoModel()
                {
                    Interval = SchedulerTimer.DairlyInterval,
                    Data = new CacheCleanerDTO
                    {
                        Path = value.LogPath,
                        IsCascading = false,
                        MaxCacheCount = 7
                    }
                });
                ModelList.Add(new ScheduleDoModel()
                {
                    Interval = SchedulerTimer.HourlyInterval / 2,
                    Data = new CacheCleanerDTO
                    {
                        Path = value.VoicePath,
                        IsCascading = false,
                        MaxCacheCount = 50
                    }
                });
            }
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
                Interval = SchedulerTimer.HourlyInterval,
                Data = new CacheCleanerDTO
                {
                    Path = "./images/RandCache/",
                    IsCascading = false,
                    MaxCacheCount = 7
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
