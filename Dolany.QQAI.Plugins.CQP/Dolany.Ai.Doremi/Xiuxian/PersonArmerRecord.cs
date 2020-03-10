using System;
using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.Ai.Doremi.Xiuxian
{
    public class PersonArmerRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public Dictionary<string, int> Armers { get; set; } = new Dictionary<string, int>();

        public Dictionary<string, int> EscapeArmers { get; set; } = new Dictionary<string, int>();

        public Dictionary<string, int> SpecialArmers { get; set; } = new Dictionary<string, int>();

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
            EscapeArmers.Remove(p => p == 0);
            SpecialArmers.Remove(p => p == 0);
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

        public void EscapeArmerGet(string name, int count = 1)
        {
            if (EscapeArmers == null)
            {
                EscapeArmers = new Dictionary<string, int>();
            }

            if (!EscapeArmers.ContainsKey(name))
            {
                EscapeArmers.Add(name, 0);
            }

            var model = AutofacSvc.Resolve<EscapeArmerSvc>()[name];
            EscapeArmers[name] = Math.Min(model.MaxContains, EscapeArmers[name] + count);
        }

        public void ConsumeEsapeArmer(string name, int count = 1)
        {
            if (EscapeArmers == null)
            {
                return;
            }

            if (!EscapeArmers.ContainsKey(name))
            {
                return;
            }

            EscapeArmers[name] = Math.Max(EscapeArmers[name] - count, 0);
        }
    }
}
