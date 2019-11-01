using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.API;

namespace Dolany.Ai.Core.Ai.Game.Lottery
{
    public class LotteryMgr
    {
        public static LotteryMgr Instance { get; } = new LotteryMgr();

        private readonly List<LotteryModel> Models;

        public const int LotteryFee = 100;

        public LotteryModel this[string Name] => Models.FirstOrDefault(p => p.Name == Name);

        private LotteryMgr()
        {
            Models = CommonUtil.ReadJsonData_NamedList<LotteryModel>("LotteryData");
        }

        public LotteryModel RandLottery()
        {
            return Models.ToDictionary(p => p, p => p.Rate).RandRated();
        }
    }

    public class LotteryModel : INamedJsonModel
    {
        public string Name { get; set; }

        public int Bonus { get; set; }

        public int Rate { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return $"{Name}\r" +
                   $"    {Description}\r" +
                   $"你获得了 {Bonus}{Emoji.钱袋}";
        }
    }
}
