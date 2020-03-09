using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Doremi.Xiuxian
{
    public class EscapeArmerSvc : IDataMgr
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
            Armers = CommonUtil.ReadJsonData_NamedList<EscapeArmerModel>("EscapeArmerData");
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
