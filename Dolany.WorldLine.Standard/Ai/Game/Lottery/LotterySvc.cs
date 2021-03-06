﻿using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.UtilityTool;

namespace Dolany.WorldLine.Standard.Ai.Game.Lottery
{
    public class LotterySvc : IDataMgr, IDependency
    {
        private List<LotteryModel> Models;

        public const int LotteryFee = 100;

        public LotteryModel this[string Name] => Models.FirstOrDefault(p => p.Name == Name);

        public LotteryModel RandLottery()
        {
            return Models.ToDictionary(p => p, p => p.Rate).RandRated();
        }

        public void RefreshData()
        {
            Models = CommonUtil.ReadJsonData_NamedList<LotteryModel>("Standard/LotteryData");
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
            return $"【{Name}】\r\n" +
                   $"    {Description}\r\n" +
                   $"你获得了 {Bonus.CurencyFormat()}";
        }
    }
}
