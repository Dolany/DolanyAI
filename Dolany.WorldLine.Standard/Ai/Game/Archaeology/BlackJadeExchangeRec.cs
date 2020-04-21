using System;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.WorldLine.Standard.Ai.Game.Archaeology
{
    /// <summary>
    /// 墨玉汇率记录
    /// </summary>
    public class BlackJadeExchangeRec : DbBaseEntity
    {
        public long GroupNum { get; set; }

        public string DateHour { get; set; }

        public int Ratio { get; set; }

        /// <summary>
        /// 获取指定群组的实时汇率
        /// </summary>
        /// <param name="GroupNum"></param>
        /// <returns></returns>
        public static int RealTimeRatio(long GroupNum)
        {
            var rec = MongoService<BlackJadeExchangeRec>.GetOnly(p => p.GroupNum == GroupNum);
            if (rec == null)
            {
                rec = new BlackJadeExchangeRec(){GroupNum = GroupNum};
                MongoService<BlackJadeExchangeRec>.Insert(rec);
            }

            if (rec.DateHour == DateTime.Now.ToString("yyyy-MM-dd:HH"))
            {
                return rec.Ratio;
            }

            rec.DateHour = DateTime.Now.ToString("yyyy-MM-dd:HH");
            rec.Ratio = Rander.RandRange(20, 350);
            MongoService<BlackJadeExchangeRec>.Update(rec);

            return rec.Ratio;
        }
    }
}
