using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.Ai.Doremi.Xiuxian
{
    public class PersonArmerRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public Dictionary<string, int> Armers { get; set; } = new Dictionary<string, int>();

        public static PersonArmerRecord Get(long QQNum)
        {
            var record = MongoService<PersonArmerRecord>.GetOnly(p => p.QQNum == QQNum);
            if (record != null)
            {
                return record;
            }

            record = new PersonArmerRecord(){QQNum = QQNum};
            MongoService<PersonArmerRecord>.Insert(record);
            return record;
        }

        public void Update()
        {
            Armers.Remove(p => p == 0);
            MongoService<PersonArmerRecord>.Update(this);
        }

        public void ArmerGet(string name, int count = 1)
        {
            if (Armers == null)
            {
                Armers = new Dictionary<string, int>();
            }

            if (!Armers.ContainsKey(name))
            {
                Armers.Add(name, 0);
            }

            Armers[name] += count;
        }
    }
}
