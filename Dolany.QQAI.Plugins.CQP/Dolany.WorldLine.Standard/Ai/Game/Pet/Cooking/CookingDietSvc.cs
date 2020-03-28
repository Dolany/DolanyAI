using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.WorldLine.Standard.OnlineStore;

namespace Dolany.WorldLine.Standard.Ai.Game.Pet.Cooking
{
    public class CookingDietSvc : IDataMgr, IDependency
    {
        public List<CookingDietModel> DietList;

        public CookingDietModel this[string name] => DietList.FirstOrDefault(p => p.Name == name);


        public CookingDietModel SuggestDiet(List<string> LearnedDiets)
        {
            var availables = DietList.Where(p => !LearnedDiets.Contains(p.Name));
            return availables.RandElement();
        }

        public void RefreshData()
        {
            DietList = CommonUtil.ReadJsonData_NamedList<CookingDietModel>("Pet/CookingDietData");
        }
    }

    public class CookingDietModel : INamedJsonModel
    {
        public string Name { get; set; }

        public int Level { get; set; }

        public string Description { get; set; }

        public Dictionary<string, int> Materials { get; set; }

        public Dictionary<string, int> Flavorings { get; set; }

        public int Exp { get; set; }

        public string[] Attributes { get; set; }

        public string ExchangeHonor { get; set; }

        public int EstimatedPrice => Materials.Sum(p => AutofacSvc.Resolve<HonorSvc>().FindItem(p.Key).Price * p.Value) + Flavorings.Sum(p => p.Value) * 20;

        public override string ToString()
        {
            var str = $"{Name}\r\n    {Description}\r\n需要等级：{Level}\r\n";
            if (!Materials.IsNullOrEmpty())
            {
                str += $"需要材料：{string.Join(",", Materials.Select(m => $"{m.Key}×{m.Value}"))}\r\n";
            }

            if (!Flavorings.IsNullOrEmpty())
            {
                str += $"需要调味料：{string.Join(",", Flavorings.Select(m => $"{m.Key}×{m.Value}"))}\r\n";
            }

            str += $"可提供经验值：{Exp}\r\n特性：{string.Join(",", Attributes)}";
            str += $"\r\n可使用【{ExchangeHonor}】兑换";
            return str;
        }
    }
}
