using System;
using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Game.Cooking
{
    public class Cooker : DbBaseEntity
    {
        public long QQNum { get; set; }

        public int Level { get; set; } = 1;

        public int Exp { get; set; }

        public int Sugar { get; set; }

        public int SAN { get; set; } = 50;

        public int MaxSAN { get; set; } = 50;

        public int Health { get; set; } = 50;

        public int MaxHealth { get; set; } = 50;

        public List<string> CookBooks { get; set; } = new List<string>();

        public Dictionary<string, int> Foods { get; set; } = new Dictionary<string, int>();

        public DateTime? RebornTime { get; set; }

        public static Cooker GetCooker(long QQNum)
        {
            var cooker = MongoService<Cooker>.GetOnly(p => p.QQNum == QQNum);
            if (cooker != null)
            {
                if (cooker.RebornTime != null && cooker.RebornTime < DateTime.Now)
                {
                    cooker.Reborn();
                }
                return cooker;
            }

            cooker = new Cooker() {QQNum = QQNum};
            MongoService<Cooker>.Insert(cooker);

            return cooker;
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
            Foods.Remove(p => p <= 0);

            MongoService<Cooker>.Update(this);
        }

        public void AddFood(string name, int count = 1)
        {
            if (Foods.ContainsKey(name))
            {
                Foods[name] += count;
            }
            else
            {
                Foods.Add(name, count);
            }
        }

        public void RemoveFood(string name, int count = 0)
        {
            if (Foods.ContainsKey(name))
            {
                Foods[name] -= count;
            }
        }
    }
}
