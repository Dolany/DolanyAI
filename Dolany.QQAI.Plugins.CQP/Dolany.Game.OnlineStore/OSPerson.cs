using System.Linq;
using Dolany.Database;

namespace Dolany.Game.OnlineStore
{
    public class OSPerson : BaseEntity
    {
        public long QQNum { get; set; }

        public int Golds { get; set; }

        public static OSPerson GetPerson(long QQNum)
        {
            var osPerson = MongoService<OSPerson>.Get(p => p.QQNum == QQNum).FirstOrDefault();
            if (osPerson == null)
            {
                osPerson = new OSPerson {QQNum = QQNum};
                MongoService<OSPerson>.Insert(osPerson);
            }

            return osPerson;
        }

        public static void GoldIncome(long QQNum, int gold)
        {
            var osPerson = MongoService<OSPerson>.Get(p => p.QQNum == QQNum).FirstOrDefault();
            if (osPerson == null)
            {
                osPerson = new OSPerson {QQNum = QQNum, Golds = gold};
                MongoService<OSPerson>.Insert(osPerson);
            }
            else
            {
                osPerson.Golds += gold;
                MongoService<OSPerson>.Update(osPerson);
            }
        }

        public static void GoldConsume(long QQNum, int gold)
        {
            var person = GetPerson(QQNum);
            person.Golds -= gold;
            MongoService<OSPerson>.Update(person);
        }
    }
}
