using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Game.WeaponWar
{
    public class WWBulletHelper
    {
        private List<BulletModel> BulletList;

        public static WWBulletHelper Instance { get; set; } = new WWBulletHelper();

        private WWBulletHelper()
        {
            var bulletDic = CommonUtil.ReadJsonData<Dictionary<string, BulletModel>>("BulletKindData");
            BulletList = bulletDic.Select(b =>
            {
                var (key, value) = b;
                value.Name = key;
                return value;
            }).ToList();
        }
    }

    public class BulletModel
    {
        public string Name { get; set; }

        public int Price { get; set; }

        public string Code { get; set; }

        public int Pack { get; set; }

        public string Aim { get; set; }
    }
}
