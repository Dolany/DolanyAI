using Dolany.Database.Sqlite;

namespace Dolany.Ai.Core.Cache
{
    using Dolany.Ai.Common;

    public class PicCacher
    {
        public static void Cache(string picUrl)
        {
            if (picUrl.Contains("/0/0-0-/0?"))
            {
                return;
            }
            SFixedSetService.Cache("PicCache", picUrl);
        }

        public static string Random()
        {
            var pics = SFixedSetService.Get<string>("PicCache");
            var pic = pics.RandElement();
            return pic;
        }
    }
}
