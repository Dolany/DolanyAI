using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.WorldLine.Standard.Ai.Game.Archaeology
{
    public class ArchSpecialCollesSvc : IDependency, IDataMgr
    {
        public List<SpecialCollesModel> SpecialColles { get; set; }

        public SpecialCollesModel this[string name] => SpecialColles.FirstOrDefault(p => p.Name == name);

        public void RefreshData()
        {
            SpecialColles = CommonUtil.ReadJsonData_NamedList<SpecialCollesModel>("Standard/Arch/SpecialCollesData");
        }
    }

    public class SpecialCollesModel : INamedJsonModel
    {
        public string Name { get; set; }

        public string Description { get;set; }

        public string BonusChannel { get; set; }

        public string PicPath { get; set; }

        public bool IsRare { get; set; }
    }
}
