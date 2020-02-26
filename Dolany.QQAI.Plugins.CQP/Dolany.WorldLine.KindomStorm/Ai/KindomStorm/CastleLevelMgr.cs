using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.WorldLine.KindomStorm.Ai.KindomStorm
{
    public class CastleLevelMgr : IDataMgr
    {
        private static DataRefresher DataRefresher => AutofacSvc.Resolve<DataRefresher>();

        public CastleLevelModel this[int level] => Levels.FirstOrDefault(p => p.Level == level);

        private List<CastleLevelModel> Levels;

        public CastleLevelMgr()
        {
            RefreshData();
            DataRefresher.Register(this);
        }

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
