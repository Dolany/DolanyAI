using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Doremi.Xiuxian
{
    public class ArmerMgr : IDataMgr
    {
        public static ArmerMgr Instance { get; } = new ArmerMgr();

        private List<ArmerModel> NormalArmerList;

        public ArmerModel this[string name]
        {
            get { return NormalArmerList.FirstOrDefault(p => p.Name == name); }
        }

        private ArmerMgr()
        {
            RefreshData();
            DataRefresher.Instance.Register(this);
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
