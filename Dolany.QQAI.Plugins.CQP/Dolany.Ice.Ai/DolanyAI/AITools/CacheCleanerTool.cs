﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO;
using Dolany.Ice.Ai.MahuaApis;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class CacheCleanerDTO
    {
        public string Path { get; set; }
        public bool IsCascading { get; set; }
        public int MaxCacheCount { get; set; }
    }

    public class CacheCleanerTool : IAITool
    {
        private int PicCleanFreq
        {
            get
            {
                var config = Utility.GetConfig(nameof(PicCleanFreq), "10");

                return int.Parse(config);
            }
        }

        private int MaxPicCache
        {
            get
            {
                var config = Utility.GetConfig("MaxPicCacheCount", "200");

                return int.Parse(config);
            }
        }

        public void Work()
        {
            JobScheduler.Instance.Add(
                PicCleanFreq * 1000,
                TimeUp,
                new CacheCleanerDTO
                {
                    Path = CodeApi.ImagePath,
                    IsCascading = false,
                    MaxCacheCount = MaxPicCache
                }
                );
            JobScheduler.Instance.Add(
                JobTimer.DairlyInterval,
                TimeUp,
                new CacheCleanerDTO
                {
                    Path = "logs/",
                    IsCascading = false,
                    MaxCacheCount = 7
                }
                );
            JobScheduler.Instance.Add(
                JobTimer.DairlyInterval,
                TimeUp,
                new CacheCleanerDTO
                {
                    Path = "RuntimeLog/",
                    IsCascading = false,
                    MaxCacheCount = 7
                }
                );
            JobScheduler.Instance.Add(
                JobTimer.HourlyInterval / 2,
                TimeUp,
                new CacheCleanerDTO
                {
                    Path = "temp/voice/",
                    IsCascading = false,
                    MaxCacheCount = 50
                }
                );
            JobScheduler.Instance.Add(
                JobTimer.HourlyInterval / 2,
                TimeUp,
                new CacheCleanerDTO
                {
                    Path = "VoiceCache/",
                    IsCascading = false,
                    MaxCacheCount = 50
                }
                );
        }

        private void TimeUp(object sender, ElapsedEventArgs e)
        {
            var timer = sender as JobTimer;
            var dto = timer.Data as CacheCleanerDTO;

            CleanCache(dto);
        }

        private void CleanCache(CacheCleanerDTO dto)
        {
            var dir = new DirectoryInfo(dto.Path);
            var cleanCount = dir.GetFiles().Count() - dto.MaxCacheCount;
            if (cleanCount <= 0)
            {
                return;
            }

            var cleanFiles = dir.GetFiles().OrderBy(f => f.CreationTime).Take(cleanCount);
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