using System.Collections.Generic;
using Dolany.Database;
using Dolany.UtilityTool;

namespace Dolany.WorldLine.Doremi.OnlineStore
{
    public class OSPerson_Doremi : DbBaseEntity
    {
        public long QQNum { get; set; }

        public int Golds { get; set; }

        public Dictionary<string, int> GiftDic { get; set; }

        public int Level { get; set; }

        public static OSPerson_Doremi GetPerson(long QQNum)
        {
            var osPerson = MongoService<OSPerson_Doremi>.GetOnly(p => p.QQNum == QQNum);
            if (osPerson != null)
            {
                return osPerson;
            }

            osPerson = new OSPerson_Doremi {QQNum = QQNum};
            MongoService<OSPerson_Doremi>.Insert(osPerson);

            return osPerson;
        }

        public static int GoldIncome(long QQNum, int gold)
        {
            var osPerson = GetPerson(QQNum);

            osPerson.Golds += gold;
            osPerson.Update();
            return osPerson.Golds;
        }

        public void Update()
        {
            GiftDic.Remove(g => g == 0);

            MongoService<OSPerson_Doremi>.Update(this);
        }
    }
}
