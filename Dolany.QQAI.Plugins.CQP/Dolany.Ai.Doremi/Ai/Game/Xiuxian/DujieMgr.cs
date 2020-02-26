using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Doremi.Ai.Game.Xiuxian
{
    public class DujieMgr : IDataMgr
    {
        public static DujieMgr Instance { get; } = new DujieMgr();

        private List<DujieQAModel> QAs;

        private DujieMgr()
        {
            RefreshData();
            //DataRefresher.Instance.Register(this);
        }

        public DujieQAModel[] RandQAs(int count)
        {
            return Rander.RandSort(QAs.ToArray()).Take(count).ToArray();
        }

        public void RefreshData()
        {
            QAs = CommonUtil.ReadJsonData_NamedList<DujieQAModel>("DujieQAData");
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
            return this.A[0] == answer;
        }
    }
}
