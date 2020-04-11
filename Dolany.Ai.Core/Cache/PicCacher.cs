using System;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Common;
using Dolany.Database.Sqlite;

namespace Dolany.Ai.Core.Cache
{
    public class PicCacher
    {
        public static void Cache(string picUrl, string format)
        {
            if (picUrl.Contains("/0/0-0-/0?"))
            {
                return;
            }
            SFixedSetService.Cache("PicCache", $"{picUrl}||{format}");
        }

        public static string Random()
        {
            while (true)
            {
                var pics = SFixedSetService.Get<string>("PicCache");
                var picCache = pics.RandElement();
                var strs = picCache.Split("||");
                var picUrl = strs[0];
                var format = strs.Length > 1 ? strs[1] : "jpg";
                var guid = Guid.NewGuid().ToString();

                var localPath = $"./images/RandCache/{guid}.{format}";
                if (!Utility.DownloadImage(picUrl, localPath))
                {
                    continue;
                }

                return localPath;
            }
        }
    }
}
