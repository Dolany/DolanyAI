using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.WorldLine.KindomStorm.Ai.KindomStorm
{
    public class CastleBuildingSvc : IDependency
    {
        public static IEnumerable<CastleBuildingModel> Buildings => new List<CastleBuildingModel>()
        {
            new CastleBuildingModel()
            {
                Name = "城镇",
                UpgradeGoldRate = 10,
                CollecRate = 10
            },
            new CastleBuildingModel()
            {
                Name = "粮仓",
                UpgradeGoldRate = 10,
                CollecRate = 100
            }
        };

        public CastleBuildingModel this[string name] => Buildings.FirstOrDefault(p => p.Name == name);
    }

    public class CastleBuildingModel
    {
        public string Name { get; set; }

        public int UpgradeGoldRate { get; set; }

        public int CollecRate { get; set; }

        public int UpgradeGoldNeed(int level)
        {
            return level * UpgradeGoldRate;
        }
    }
}
