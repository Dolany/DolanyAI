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

        public static PetRecord Get(long QQNum)
        {
            var pet = MongoService<PetRecord>.GetOnly(p => p.QQNum == QQNum);
            if (pet != null)
            {
                return pet;
            }

            pet = new PetRecord(){QQNum = QQNum, PetNo = "Neptune", Name = "涅普", Level = 1, PicPath = "./images/Pet/Neptune/Default.jpg"};
            MongoService<PetRecord>.Insert(pet);
            return pet;
        }

        public void Update()
        {
            MongoService<PetRecord>.Update(this);
        }
    }
}
