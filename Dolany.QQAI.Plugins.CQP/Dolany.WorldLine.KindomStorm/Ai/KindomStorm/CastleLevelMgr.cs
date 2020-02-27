using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.WorldLine.KindomStorm.Ai.KindomStorm
{
    public class CastleLevelMgr : IDataMgr, IDependency
    {
        public CastleLevelModel this[int level] => Levels.FirstOrDefault(p => p.Level == level);

        private List<CastleLevelModel> Levels;

        public void RefreshData()
        {
            Levels = CommonUtil.ReadJsonData<List<CastleLevelModel>>("CastleLevelData");
        }
    }

    public class CastleLevelModel
    {
        public int Level { get; set; }

        public int Gold { get; set; }

        public int SoldierLimit { get; set; }
    }
}
