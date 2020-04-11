using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.WorldLine.Standard.OnlineStore
{
    public partial class OSPerson : DbBaseEntity
    {
        public long QQNum { get; set; }

        public int Golds { get; set; }

        public int Diamonds { get; set; }

        public Dictionary<string, int> GiftDic { get; set; } = new Dictionary<string, int>();

        public List<string> HonorNames { get; set; } = new List<string>();

        public static OSPerson GetPerson(long QQNum)
        {
            var osPerson = MongoService<OSPerson>.GetOnly(p => p.QQNum == QQNum);
            if (osPerson == null)
            {
                osPerson = new OSPerson {QQNum = QQNum};
                MongoService<OSPerson>.Insert(osPerson);
            }

            if (osPerson.HonorNames == null)
            {
                osPerson.HonorNames = new List<string>();
            }

            if (osPerson.Level != 0)
            {
                return osPerson;
            }

            osPerson.Level = 1;
            osPerson.MaxHP = 50;

            return osPerson;
        }

        public void GiftIncome(string GiftName, int count = 1)
        {
            if (GiftDic == null)
            {
                GiftDic = new Dictionary<string, int>();
            }

            if (GiftDic.ContainsKey(GiftName))
            {
                GiftDic[GiftName] += count;
            }

            GiftDic.Add(GiftName, count);
        }

        public static void GoldIncome(long QQNum, int gold)
        {
            var osPerson = GetPerson(QQNum);

            osPerson.Golds += gold;
            osPerson.Update();
        }

        public static int GoldConsume(long QQNum, int gold)
        {
            var person = GetPerson(QQNum);
            person.Golds -= gold;
            person.Update();

            return person.Golds;
        }

        public void Update()
        {
            GiftDic.Remove(g => g == 0);

            MongoService<OSPerson>.Update(this);
        }
    }
}
