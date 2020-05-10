using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;
using Dolany.UtilityTool;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Dolany.WorldLine.Standard.Ai.Game.Pet
{
    public class PetRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public string PetNo { get; set; }

        public string Name { get; set; }

        public int Level { get; set; }

        public int Exp { get; set; }

        public string PicPath { get; set; }

        public string Attribute { get; set; }

        public int RemainSkillPoints { get; set; }

        public Dictionary<string, int> Skills { get; set; } = new Dictionary<string, int>();

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? LastFeedTime { get; set; }

        public static PetRecord Get(long QQNum)
        {
            var pet = MongoService<PetRecord>.GetOnly(p => p.QQNum == QQNum);
            if (pet != null)
            {
                return pet;
            }

            var aimPath = $"./images/Custom/Pet/{QQNum}.jpg";
            File.Copy("./images/Pet/Neptune/Default.jpg", aimPath, true);
            pet = new PetRecord(){QQNum = QQNum, PetNo = "Neptune", Name = "涅普", Level = 1, PicPath = aimPath};
            MongoService<PetRecord>.Insert(pet);

            return pet;
        }

        public void Update()
        {
            MongoService<PetRecord>.Update(this);
        }

        public static Dictionary<int, int> LevelAnalyze()
        {
            var pets = MongoService<PetRecord>.Get();
            return pets.GroupBy(p => p.Level).ToDictionary(p => p.Key, p => p.Count()).OrderByDescending(p => p.Key).ToDictionary(p => p.Key, p => p.Value);
        }

        public static IEnumerable<PetRecord> LevelTop(int count)
        {
            var sort = Builders<PetRecord>.Sort.Descending(p => p.Level).Descending(p => p.Exp);
            return MongoService<PetRecord>.GetCollection().Find(p => true).Sort(sort).Limit(count).ToList();
        }

        public void SkillReset()
        {
            if (Skills.IsNullOrEmpty())
            {
                return;
            }

            var sum = 0;

            for (var i = 0; i < Skills.Count; i++)
            {
                var key = Skills.Keys.ElementAt(i);
                sum += Skills[key] - 1;
                Skills[key] = 1;
            }

            RemainSkillPoints += sum;
        }
    }
}
