using System.Collections.Generic;
using Dolany.Database;

namespace Dolany.Game.FreedomMagic
{
    public class FMPlayer : BaseEntity
    {
        public long QQNum { get; set; }

        public List<FMMagic> Magics { get; set; }

        public int MagicVolume { get; set; }

        public int Level { get; set; }

        public int CurExp { get; set; }

        public int MaxMagicLevel
        {
            get
            {
                var levelModel = FMLevelHelper.Instance[Level];
                return levelModel.MaxMagicLevel;
            }
        }

        public bool IsMagicFull => Magics.Count >= MagicVolume;

        public FMMagic LearnMagic(string name, string singWords, int magicLevel)
        {
            var magic = new FMMagic
            {
                Name = name,
                SingWords = singWords,
                Effects = FMEffectHelper.Instance.GetRandEffect(magicLevel)
            };
            Magics.Add(magic);

            return magic;
        }

        public void ForgetMagic(string name)
        {
            Magics.RemoveAll(m => m.Name == name);
        }

        public static FMPlayer Create(long QQNum)
        {
            return new FMPlayer()
            {
                QQNum = QQNum,
                Magics = new List<FMMagic>(),
                MagicVolume = 10,
                Level = 1,
                CurExp = 0
            };
        }
    }
}
