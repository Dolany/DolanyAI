using System;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.WorldLine.Standard.Ai.Game.Lottery
{
    public class LotteryRecord : DbBaseEntity
    {
        public string DataStr { get; set; }

        public int Count { get; set; }

        public int TotalPlus { get; set; }

        public int TotalMinus { get; set; }

        public override string ToString()
        {
            return $"共计开箱子 {Count} 次\r总盈利 {TotalPlus.CurencyFormat()}\r总亏损 {TotalMinus.CurencyFormat()}\r合计 {(TotalPlus > TotalMinus ? "+" : "")}{(TotalPlus - TotalMinus).CurencyFormat()}";
        }

        public static LotteryRecord GetToday()
        {
            var dateStr = DateTime.Now.ToString("yyyyMMdd");
            var record = MongoService<LotteryRecord>.GetOnly(p => p.DataStr == dateStr);
            if (record != null)
            {
                return record;
            }

            record = new LotteryRecord()
            {
                DataStr = dateStr
            };

            MongoService<LotteryRecord>.Insert(record);
            return record;
        }

        public static LotteryRecord GetYesterday()
        {
            var dateStr = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
            var record = MongoService<LotteryRecord>.GetOnly(p => p.DataStr == dateStr);
            if (record != null)
            {
                return record;
            }

            record = new LotteryRecord()
            {
                DataStr = dateStr
            };

            MongoService<LotteryRecord>.Insert(record);
            return record;
        }

        public static void Record(int absBonus)
        {
            var todayRec = GetToday();
            todayRec.Count++;
            if (absBonus > 0)
            {
                todayRec.TotalPlus += absBonus;
            }
            else
            {
                todayRec.TotalMinus += Math.Abs(absBonus);
            }
            todayRec.Update();
        }

        public void Update()
        {
            MongoService<LotteryRecord>.Update(this);
        }
    }
}
