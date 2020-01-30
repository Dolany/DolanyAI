using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.WorldLine.KindomStorm.Ai.KindomStorm
{
    public class TownLevelMgr
    {
        public static TownLevelMgr Instance { get; } = new TownLevelMgr();

        private readonly List<TownLevelModel> Levels;

        private TownLevelModel this[int level] => Levels.FirstOrDefault(p => p.Level == level);

        private TownLevelMgr()
        {
            Levels = CommonUtil.ReadJsonData<List<TownLevelModel>>("TownLevelData");
        }
    }

    public class TownLevelModel
    {
        public int Level { get; set; }

        public int Gold { get; set; }

        public int PerDayGen { get; set; }
    }
}
