using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.WorldLine.Standard.Ai.Game.Archaeology
{
    public class ArchaeologistLevelSvc : IDataMgr, IDependency
    {
        private List<ArchaeologistLevelModel> Levels;

        public ArchaeologistLevelModel this[int level] => Levels.FirstOrDefault(p => p.Level == level);

        public void RefreshData()
        {
            Levels = CommonUtil.ReadJsonData_NamedList<ArchaeologistLevelModel>("Arch/ArchLevelData");
        }
    }

    public class ArchaeologistLevelModel : INamedJsonModel
    {
        public string Name { get; set; }

        public int PowerGain { get; set; }

        public int Exp { get; set; }

        public int Level => int.Parse(Name);
    }
}
