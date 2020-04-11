using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.WorldLine.Doremi.Ai.Game.Xiuxian
{
    public class DujieSvc : IDataMgr
    {
        private List<DujieQAModel> QAs;

        private static DataRefreshSvc DataRefreshSvc => AutofacSvc.Resolve<DataRefreshSvc>();

        public DujieSvc()
        {
            RefreshData();
            DataRefreshSvc.Register(this);
        }

        public DujieQAModel[] RandQAs(int count)
        {
            return Rander.RandSort(QAs.ToArray()).Take(count).ToArray();
        }

        public void RefreshData()
        {
            QAs = CommonUtil.ReadJsonData_NamedList<DujieQAModel>("Doremi/DujieQAData");
        }
    }

    public class DujieQAModel : INamedJsonModel
    {
        public string Name { get; set; }

        public string Q { get; set; }

        public string[] A { get; set; }

        public string[] RandAs => Rander.RandSort(A.ToArray());

        public bool IsCorrect(string answer)
        {
            return A[0] == answer;
        }
    }
}
