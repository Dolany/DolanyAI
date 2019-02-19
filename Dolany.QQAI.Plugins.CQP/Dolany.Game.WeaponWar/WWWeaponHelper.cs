using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Game.WeaponWar
{
    public class WWWeaponHelper
    {
        private readonly List<WeaponModel> WeaponList;

        public static WWWeaponHelper Instance { get; set; } = new WWWeaponHelper();

        private WWWeaponHelper()
        {
            var weaponDic = CommonUtil.ReadJsonData<Dictionary<string, WeaponModel>>("WeaponData");
            WeaponList = weaponDic.Select(w =>
            {
                var (key, value) = w;
                value.Name = key;
                return value;
            }).ToList();
        }

        public WeaponModel FindWeapon(string code)
        {
            return WeaponList.FirstOrDefault(w => w.Code == code);
        }
    }

    public class WeaponModel
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public int HP { get; set; }

        public int Attack { get; set; }

        public int CD { get; set; }

        public int Weight { get; set; }

        public int Volume { get; set; }

        public IList<string> BulletKind { get; set; }

        public WWCombineNeed CombineNeed { get; set; }
    }
}
