using System.Collections.Generic;
using System.IO;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Game.Pet
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

        public Dictionary<string, int> Skills { get; set; }

        public static PetRecord Get(long QQNum)
        {
            var pet = MongoService<PetRecord>.GetOnly(p => p.QQNum == QQNum);
            if (pet != null)
            {
                return pet;
            }

            var aimPath = $"./images/Custom/Pet/{QQNum}.jpg";
            File.Copy("./images/Pet/Neptune/Default.jpg", aimPath);
            pet = new PetRecord(){QQNum = QQNum, PetNo = "Neptune", Name = "涅普", Level = 1, PicPath = aimPath};
            MongoService<PetRecord>.Insert(pet);
            return pet;
        }

        public void Update()
        {
            MongoService<PetRecord>.Update(this);
        }
    }
}
