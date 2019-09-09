using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Doremi.Xiuxian
{
    public class ArmerMgr
    {
        public static ArmerMgr Instance { get; } = new ArmerMgr();

        private readonly List<ArmerModel> NormalArmerList;

        private readonly List<ArmerModel> RatedNormalList;

        public ArmerModel this[string name]
        {
            get { return NormalArmerList.FirstOrDefault(p => p.Name == name); }
        }

        private ArmerMgr()
        {
            NormalArmerList = CommonUtil.ReadJsonData_NamedList<ArmerModel>("ArmerData");

            RatedNormalList = new List<ArmerModel>();
            foreach (var model in NormalArmerList)
            {
                for (var i = 0; i < model.Rate; i++)
                {
                    RatedNormalList.Add(model);
                }
            }
        }

        public IEnumerable<ArmerModel> GetRandArmers(int count)
        {
            var resultList = new List<ArmerModel>();
            var rand = Rander.RandSort(RatedNormalList.ToArray());
            for (var i = 0; i < rand.Length && resultList.Count < count; i++)
            {
                if (resultList.Any(p => p.Name == rand[i].Name))
                {
                    continue;
                }

                resultList.Add(rand[i]);
            }

            return resultList;
        }

        public ArmerModel RandTagArmer(string tagName)
        {
            return NormalArmerList.Where(p => p.ArmerTag == tagName).ToDictionary(p => p, p => p.Rate).RandRated();
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
    }
}
