using System;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Game.Pet
{
    public class PetEnduranceRecord : DbBaseEntity
    {
        public long QQNum { get;set; }

        public string DateStr { get; set; }

        public int ConsumeTotal { get; set; }

        public static PetEnduranceRecord Get(long QQNum)
        {
            var record = MongoService<PetEnduranceRecord>.GetOnly(p => p.QQNum == QQNum);
            if (record == null)
            {
                record = new PetEnduranceRecord()
                {
                    QQNum = QQNum
                };
                MongoService<PetEnduranceRecord>.Insert(record);
            }

            var todayStr = DateTime.Now.ToString("yyyyMMdd");
            if (record.DateStr == todayStr)
            {
                return record;
            }

            record.ConsumeTotal = 0;
            record.DateStr = todayStr;

            return record;
        }

        public void Update()
        {
            MongoService<PetEnduranceRecord>.Update(this);
        }
    }
}
