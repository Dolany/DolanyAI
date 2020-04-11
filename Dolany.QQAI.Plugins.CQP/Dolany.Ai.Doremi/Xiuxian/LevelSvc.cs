using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Doremi.Xiuxian
{
    public class LevelSvc : IDataMgr, IDependency
    {
        private List<LevelDataModel> LevelDataList;

        public int TopLevel => LevelDataList.Max(p => p.Level);

        public LevelDataModel GetByLevel(int level)
        {
            return LevelDataList.FirstOrDefault(p => p.Level == level);
        }

        public void RefreshData()
        {
            LevelDataList = CommonUtil.ReadJsonData_NamedList<LevelDataModel>("Doremi/LevelData");
        }
    }

    public class LevelDataModel : INamedJsonModel
    {
        public string Name { get; set; }

        public int Level { get; set; }

        public int Exp { get; set; }

        public int HP { get; set; }

        public int Atk { get; set; }
    }
}
