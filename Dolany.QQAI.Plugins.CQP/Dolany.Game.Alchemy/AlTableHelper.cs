using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Game.Alchemy
{
    public class AlTableHelper
    {
        private List<AlTableDataModel> DataList;

        public static AlTableHelper Instance { get; } = new AlTableHelper();

        private AlTableHelper()
        {
            var dataDic = CommonUtil.ReadJsonData<Dictionary<int, AlTableDataModel>>("AlTableData");
            DataList = dataDic.Select(d =>
            {
                var (key, value) = d;
                value.Level = key;
                return value;
            }).ToList();
        }

        public int MaxLevel => DataList.Max(d => d.Level);

        public AlTableDataModel this[int level] => DataList.First(d => d.Level == level);
    }

    public class AlTableDataModel
    {
        public int Level { get; set; }

        public int MaxFire { get; set; }

        public int MaxWater { get; set; }

        public int MaxSolid { get; set; }

        public int MaxThurnder { get; set; }

        public AlCombineNeed UpgradeNeed { get; set; }
    }
}
