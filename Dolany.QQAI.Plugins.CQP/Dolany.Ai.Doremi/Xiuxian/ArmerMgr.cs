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

        public List<ArmerModel> GetRandArmers(int count)
        {
            var resultList = new List<ArmerModel>();
            var rand = CommonUtil.RandSort(RatedNormalList.ToArray());
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
    }
}
