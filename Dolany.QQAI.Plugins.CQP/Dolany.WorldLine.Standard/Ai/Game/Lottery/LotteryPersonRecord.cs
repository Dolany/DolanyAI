using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.WorldLine.Standard.Ai.Game.Lottery
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
            var LotteryMgr = AutofacSvc.Resolve<LotterySvc>();
            var ordered = LotteryDic.Select(p => new {Model = LotteryMgr[p.Key], Count = p.Value}).OrderByDescending(p => p.Model.Bonus).ToList();
            var str = string.Join("\r\n", ordered.Select(p => $"{p.Model.Name}({p.Model.Bonus.CurencyFormat()})*{p.Count}次"));
            str += $"\r\n总计{ordered.Sum(p => p.Count)}次";
            str += $"\r\n总盈亏{ordered.Sum(p => (p.Model.Bonus - LotterySvc.LotteryFee) * p.Count).CurencyFormat()}";

            return str;
        }
    }
}
