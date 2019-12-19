using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Game.Lottery
{
    public class LotteryPersonRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public Dictionary<string, int> LotteryDic { get; set; } = new Dictionary<string, int>();

        public static LotteryPersonRecord Get(long QQNum)
        {
            var record = MongoService<LotteryPersonRecord>.GetOnly(p => p.QQNum == QQNum);
            if (record != null)
            {
                return record;
            }

            record = new LotteryPersonRecord()
            {
                QQNum = QQNum
            };
            MongoService<LotteryPersonRecord>.Insert(record);
            return record;
        }

        public void Update()
        {
            MongoService<LotteryPersonRecord>.Update(this);
        }

        public void AddLottery(string LotteryName)
        {
            if (!LotteryDic.ContainsKey(LotteryName))
            {
                LotteryDic.Add(LotteryName, 0);
            }

            LotteryDic[LotteryName]++;
        }

        public override string ToString()
        {
            var ordered = LotteryDic.Select(p => new {Model = LotteryMgr.Instance[p.Key], Count = p.Value}).OrderByDescending(p => p.Model.Bonus).ToList();
            var str = string.Join("\r", ordered.Select(p => $"{p.Model.Name}*{p.Count}次"));
            str += $"\r总计{ordered.Sum(p => p.Count)}次";
            str += $"\r总盈亏{ordered.Sum(p => (p.Model.Bonus - LotteryMgr.LotteryFee) * p.Count)}{Emoji.钱袋}";

            return str;
        }
    }
}
