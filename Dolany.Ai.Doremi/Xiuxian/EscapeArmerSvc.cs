using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.UtilityTool;

namespace Dolany.WorldLine.Doremi.Xiuxian
{
    public class EscapeArmerSvc : IDataMgr, IDependency
    {
        private List<EscapeArmerModel> Armers;

        public EscapeArmerModel this[string Name] => Armers.FirstOrDefault(p => p.Name == Name);

        private static DataRefreshSvc DataRefreshSvc => AutofacSvc.Resolve<DataRefreshSvc>();

        public EscapeArmerSvc()
        {
            RefreshData();
            DataRefreshSvc.Register(this);
        }

        public EscapeArmerModel RandArmer()
        {
            return Armers.ToDictionary(p => p, p => p.Rate).RandRated();
        }

        public void RefreshData()
        {
            Armers = CommonUtil.ReadJsonData_NamedList<EscapeArmerModel>("Doremi/EscapeArmerData");
        }
    }

    public class EscapeArmerModel : INamedJsonModel
    {
        public string Name { get; set; }

        public int Rate { get; set; }

        public int MaxContains { get; set; }

        public int SuccessRate { get; set; }
    }
}
