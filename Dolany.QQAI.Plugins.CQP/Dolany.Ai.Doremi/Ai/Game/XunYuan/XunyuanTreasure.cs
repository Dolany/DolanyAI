using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Doremi.OnlineStore;
using Dolany.Ai.Doremi.Xiuxian;

namespace Dolany.Ai.Doremi.Ai.Game.XunYuan
{
    public class XunyuanTreasure
    {
        public int Golds { get; set; }

        public Dictionary<string, int> NormalArmers { get;set; } = new Dictionary<string, int>();

        public Dictionary<string, int> EscapeArmers { get; set; } = new Dictionary<string, int>();

        public void Clear()
        {
            NormalArmers.Clear();
            EscapeArmers.Clear();

            Golds = 0;
        }

        public void Simplify()
        {
            NormalArmers.Remove(p => p == 0);
            EscapeArmers.Remove(p => p == 0);
        }

        public void AddArmer(string name)
        {
            if (!NormalArmers.ContainsKey(name))
            {
                NormalArmers.Add(name, 0);
            }

            NormalArmers[name]++;
        }

        public void AddEscape(string name)
        {
            if (!EscapeArmers.ContainsKey(name))
            {
                EscapeArmers.Add(name, 0);
            }

            EscapeArmers[name]++;
        }

        public override string ToString()
        {
            var dic = new Dictionary<string, int>();
            Simplify();

            if (!NormalArmers.IsNullOrEmpty())
            {
                foreach (var (Name, Count) in NormalArmers)
                {
                    dic.Add(Name, Count);
                }
            }

            if (!EscapeArmers.IsNullOrEmpty())
            {
                foreach (var (Name, Count) in EscapeArmers)
                {
                    dic.Add(Name, Count);
                }
            }

            if (Golds != 0)
            {
                dic.Add("金币", Golds);
            }

            return dic.IsNullOrEmpty() ? "" : string.Join("\r", dic.Select(p => $"{p.Key} * {p.Value}"));
        }

        public void SaveToPerson(long QQNum)
        {
            Simplify();

            var armerRecord = PersonArmerRecord.Get(QQNum);
            foreach (var (name, count) in NormalArmers)
            {
                armerRecord.ArmerGet(name, count);
            }

            foreach (var (name, count) in EscapeArmers)
            {
                armerRecord.EscapeArmerGet(name, count);
            }

            var osPerson = OSPerson.GetPerson(QQNum);
            osPerson.Golds += Golds;

            armerRecord.Update();
            osPerson.Update();
        }

        public XunyuanTreasure[] Split()
        {
            Simplify();

            var first = new XunyuanTreasure();
            var second = new XunyuanTreasure();

            foreach (var (key, value) in NormalArmers)
            {
                var randCount = Rander.RandInt(value + 1);
                first.NormalArmers.Add(key, randCount);
                second.NormalArmers.Add(key, value - randCount);
            }

            foreach (var (key, value) in EscapeArmers)
            {
                var randCount = Rander.RandInt(value + 1);
                first.EscapeArmers.Add(key, randCount);
                second.EscapeArmers.Add(key, value - randCount);
            }

            var randGold = Rander.RandInt(Golds + 1);
            first.Golds = randGold;
            second.Golds = Golds - randGold;

            return new[] {first, second};
        }
    }
}
