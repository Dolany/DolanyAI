using System;
using System.Collections.Generic;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Game.Alchemy
{
    public class Alchemier : DbBaseEntity
    {
        public long QQNum { get; set; }

        public int Level { get; set; } = 1;

        public int Exp { get; set; }

        public int Sugar { get; set; }

        public int SAN { get; set; } = 50;

        public int MaxSAN { get; set; } = 50;

        public int Health { get; set; } = 50;

        public int MaxHealth { get; set; } = 50;

        public List<string> AlchemyBooks { get; set; } = new List<string>();

        public DateTime? RebornTime { get; set; }

        public static Alchemier GetAlchemier(long QQNum)
        {
            var alchemier = MongoService<Alchemier>.GetOnly(p => p.QQNum == QQNum);
            if (alchemier != null)
            {
                if (alchemier.RebornTime != null && alchemier.RebornTime < DateTime.Now)
                {
                    alchemier.Reborn();
                }
                return alchemier;
            }

            alchemier = new Alchemier() {QQNum = QQNum};
            MongoService<Alchemier>.Insert(alchemier);

            return alchemier;
        }

        public void Reborn()
        {
            Sugar = 0;
            SAN = MaxSAN;
            Health = MaxHealth;

            RebornTime = null;
        }

        public void LevelUp()
        {
            Level++;
            MaxSAN += 5;
            MaxHealth += 5;

            Reborn();
        }

        public void Update()
        {
            MongoService<Alchemier>.Update(this);
        }
    }
}
