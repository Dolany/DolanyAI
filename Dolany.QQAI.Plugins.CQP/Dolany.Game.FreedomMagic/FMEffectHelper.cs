using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Game.FreedomMagic
{
    public class FMEffectHelper
    {
        private List<FMEffect> EffectsList { get; }

        private Dictionary<int, FMEffect[]> EffectLevelDic { get; }

        private const int SingleRate = 70;

        public static FMEffectHelper Instance { get; } = new FMEffectHelper();

        private FMEffectHelper()
        {
            var data = CommonUtil.ReadJsonData<Dictionary<string, FMEffect[]>>("effectData");
            EffectsList = data.SelectMany(d =>
            {
                var (key, effects) = d;
                foreach (var effect in effects)
                {
                    effect.Type = key;
                }

                return effects;
            }).ToList();

            EffectLevelDic = EffectsList.GroupBy(p => p.Level).ToDictionary(p => p.Key, p => p.ToArray());
        }

        public List<FMEffect> GetRandEffect(int totalLevel)
        {
            if (totalLevel == 1 || CommonUtil.RandInt(100) < SingleRate)
            {
                return new List<FMEffect>(){GetRandEffect(totalLevel, null)};
            }

            var firstLevel = totalLevel / 2;
            var secondLevel = totalLevel - firstLevel;

            var firstEffect = GetRandEffect(firstLevel, null);
            var secondEffect = GetRandEffect(secondLevel, new[] {firstEffect.Type});

            return new List<FMEffect>() {firstEffect, secondEffect};
        }

        private FMEffect GetRandEffect(int level, string[] skipEffects)
        {
            var levelList = EffectLevelDic[level];
            if (skipEffects.IsNullOrEmpty())
            {
                return levelList[CommonUtil.RandInt(EffectsList.Count)];
            }

            var list = levelList.Where(e => skipEffects.Contains(e.Type)).ToList();
            return list[CommonUtil.RandInt(list.Count)];
        }
    }
}
