using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Doremi.Xiuxian
{
    public class ArmerMgr : IDataMgr
    {
        private List<ArmerModel> NormalArmerList;

        private static DataRefreshSvc DataRefreshSvc => AutofacSvc.Resolve<DataRefreshSvc>();

        public ArmerModel this[string name]
        {
            get { return NormalArmerList.FirstOrDefault(p => p.Name == name); }
        }

        public ArmerMgr()
        {
            RefreshData();
            DataRefreshSvc.Register(this);
        }

        public IEnumerable<ArmerModel> GetRandArmers(int count)
        {
            return NormalArmerList.ToDictionary(p => p, p => p.Rate).RandRated(count);
        }

        public ArmerModel RandTagArmer(string tagName)
        {
            return NormalArmerList.Where(p => p.ArmerTag == tagName || tagName == "All").ToDictionary(p => p, p => p.Rate).RandRated();
        }

        public int CountHP(Dictionary<string, int> ArmerList)
        {
            if (ArmerList.IsNullOrEmpty())
            {
                return 0;
            }

            var modelsDic = ArmerList.ToDictionary(p => this[p.Key], p => p.Value);
            return modelsDic.Where(p => p.Key.Kind == "Shield").Sum(p => p.Key.Value * p.Value);
        }

        public int CountAtk(Dictionary<string, int> ArmerList)
        {
            if (ArmerList.IsNullOrEmpty())
            {
                return 0;
            }

            var modelsDic = ArmerList.ToDictionary(p => this[p.Key], p => p.Value);
            return modelsDic.Where(p => p.Key.Kind == "Weapon").Sum(p => p.Key.Value * p.Value);
        }

        public void RefreshData()
        {
            NormalArmerList = CommonUtil.ReadJsonData_NamedList<ArmerModel>("ArmerData");
        }
    }
}
