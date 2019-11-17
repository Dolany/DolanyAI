using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Ai.Game.Pet.Cooking
{
    public class CookingDietMgr
    {
        public static CookingDietMgr Instance { get; } = new CookingDietMgr();

        private readonly List<CookingDietModel> DietList;

        public CookingDietModel this[string name] => DietList.FirstOrDefault(p => p.Name == name);

        private CookingDietMgr()
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

        public override string ToString()
        {
            var str = $"{Name}\r    {Description}\r需要等级：{Level}\r";
            if (!Materials.IsNullOrEmpty())
            {
                str += $"需要材料：{string.Join(",", Materials.Select(m => $"{m.Key}×{m.Value}"))}\r";
            }

            if (!Flavorings.IsNullOrEmpty())
            {
                str += $"需要调味料：{string.Join(",", Flavorings.Select(m => $"{m.Key}×{m.Value}"))}\r";
            }

            str += $"可提供经验值：{Exp}\r特性：{string.Join(",", Attributes)}";
            return str;
        }
    }
}
