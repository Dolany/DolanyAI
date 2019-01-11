using JetBrains.Annotations;

namespace Dolany.Game.IncantationGame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Database.Incantation;
    using Effect;

    public class MagicHelper
    {
        [NotNull]
        public static IncaMagic CreateNewMagic(long QQNum, string Name, string Word, int ValidCount, EffectKindEnum AimKind)
        {
            var magic = new IncaMagic { Name = Name, QQNum = QQNum, Word = Word };
            var magicLevel = ParseMagicLevel(ValidCount);
            magic.SuccessRate = ParseSRate(magicLevel);
            magic.CD = magicLevel;
            magic.ChantDuring = (magicLevel + 1) / 2 - 1;

            var effectTotalLv = ParseEffectTotalLv(magicLevel);
            var effects = ParseEffects(effectTotalLv, AimKind);
            SaveToDb(magic, effects);

            return magic;
        }

        private static void SaveToDb(IncaMagic magic, IEnumerable<IEffect> effects)
        {
            using (var db = new IncaDatabase())
            {
                db.IncaMagic.Add(magic);

                foreach (var effect in effects)
                {
                    db.IncaEffect.Add(new IncaEffect { Effect = effect.Name, MagicId = magic.Id, Value = effect.Value });
                }

                db.SaveChanges();
            }
        }

        public static int ValidCount(string Word)
        {
            return string.IsNullOrEmpty(Word) ? 0 : Word.Distinct().Count();
        }

        private static int ParseMagicLevel(int ValidCount)
        {
            if (ValidCount < 10)
            {
                return 1;
            }

            if (ValidCount < 15)
            {
                return 2;
            }

            if (ValidCount < 20)
            {
                return 3;
            }

            if (ValidCount < 25)
            {
                return 4;
            }

            return 5;
        }

        private static int ParseSRate(int MagicLevel)
        {
            var rand = new Random();

            switch (MagicLevel)
            {
                case 1:
                    return rand.Next(10) + 90;
                case 2:
                    return rand.Next(10) + 80;
                case 3:
                    return rand.Next(10) + 70;
                case 4:
                    return rand.Next(10) + 60;
                default:
                    return rand.Next(10) + 50;
            }
        }

        private static int ParseEffectTotalLv(int MagicLevel)
        {
            var rand = new Random();

            switch (MagicLevel)
            {
                case 1:
                    return 1;
                case 2:
                    return 2;
                case 3:
                    return rand.Next(1) + 3;
                case 4:
                    return rand.Next(1) + 5;
                default:
                    return 1;
            }
        }

        private static IEnumerable<IEffect> ParseEffects(int TotalLv, EffectKindEnum AimKind)
        {
            var rand = new Random();
            var randNum = rand.Next(101);
            if (randNum <= 70 || TotalLv <= 2)
            {
                return new[] { ParseEffect(TotalLv, AimKind) };
            }

            var effects = EffectFactory.AllEffects.Where(e => e.Kind != AimKind).ToList();
            var effect = effects.ElementAt(rand.Next(effects.Count));
            var otherKind = effect.Kind;

            return new[] { ParseEffect(TotalLv - 2, AimKind), ParseEffect(2, otherKind) };
        }

        private static IEffect ParseEffect(int level, EffectKindEnum AimKind)
        {
            var rand = new Random();

            var effects = EffectFactory.AllEffects.Where(e => e.Kind == AimKind).ToList();
            var effect = effects.ElementAt(rand.Next(effects.Count));

            return effect.CreateNew(level);
        }
    }
}
