﻿using System;
using Dolany.Ai.Core.API;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Game.Shopping
{
    public class LotteryRecord : DbBaseEntity
    {
        public string DataStr { get; set; }

        public int Count { get; set; }

        public int TotalPlus { get; set; }

        public int TotalMinus { get; set; }

        public override string ToString()
        {
            return $"共计买彩票 {Count} 次\r总盈利 {TotalPlus}{Emoji.钱袋}\r总亏损 {TotalMinus}{Emoji.钱袋}\r合计 {(TotalPlus > TotalMinus ? "+" : "")}{TotalPlus - TotalMinus}{Emoji.钱袋}";
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

        public void Update()
        {
            MongoService<LotteryRecord>.Update(this);
        }
    }
}
