using System;
using System.Collections.Generic;

namespace Dolany.Database.Ai
{
    public class OSPersonBuff : DbBaseEntity
    {
        public long QQNum { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ExpiryTime { get; set; }
        public bool IsPositive { get; set; }
        public int Data { get; set; } = 1;
        public long Source { get; set; }

        public static bool CheckBuff(long QQNum, string Name)
        {
            return MongoService<OSPersonBuff>.GetOnly(p => p.QQNum == QQNum && p.Name == Name) != null;
        }

        public static List<OSPersonBuff> GetByIsPositive(long QQNum, bool IsPositive)
        {
            return MongoService<OSPersonBuff>.Get(p => p.QQNum == QQNum && p.IsPositive == IsPositive);
        }

        public static List<OSPersonBuff> Get(long QQNum)
        {
            return MongoService<OSPersonBuff>.Get(p => p.QQNum == QQNum);
        }

        public void Add()
        {
            var buff = MongoService<OSPersonBuff>.GetOnly(p => p.QQNum == QQNum && p.Name == Name);
            if (buff == null)
            {
                MongoService<OSPersonBuff>.Insert(this);
                return;
            }

            buff.ExpiryTime = ExpiryTime;
            buff.Data = Data;
            buff.Source = Source;
            MongoService<OSPersonBuff>.Update(buff);
        }

        public void Remove()
        {
            MongoService<OSPersonBuff>.Delete(this);
        }

        public static void RemoveAll(long QQNum)
        {
            MongoService<OSPersonBuff>.DeleteMany(p => p.QQNum == QQNum);
        }

        public static void Remove(long QQNum, string BuffName)
        {
            MongoService<OSPersonBuff>.DeleteMany(p => p.QQNum == QQNum && p.Name == BuffName);
        }
    }
}
