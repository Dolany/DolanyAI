using System;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Vip
{
    public class DailyVipGoodsRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public string Date { get; set; }

        public string[] GoodsName { get; set; }

        public static DailyVipGoodsRecord GetToday(long QQNum)
        {
            var record = MongoService<DailyVipGoodsRecord>.GetOnly(p => p.QQNum == QQNum);
            if (record == null)
            {
                record = new DailyVipGoodsRecord()
                {
                    QQNum = QQNum
                };
                MongoService<DailyVipGoodsRecord>.Insert(record);
            }

            var dateStr = DateTime.Now.ToString("yyyy-MM-dd");
            if (record.Date == dateStr)
            {
                return record;
            }

            record.Date = dateStr;
            record.GoodsName = DailyVipShopMgr.Instance.RandGoods(5);

            return record;
        }

        public void Update()
        {
            MongoService<DailyVipGoodsRecord>.Update(this);
        }
    }
}
