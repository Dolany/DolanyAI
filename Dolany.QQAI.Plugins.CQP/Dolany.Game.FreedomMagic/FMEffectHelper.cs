using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Game.FreedomMagic
{
    public class FMEffectHelper
    {
        private List<FMEffect> EffectsList { get; }

        private Dictionary<int, FMEffect[]> LevelDic { get; }

        private int SingleRate = 70;

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

            LevelDic = EffectsList.GroupBy(p => p.Level).ToDictionary(p => p.Key, p => p.ToArray());
        }

        public List<FMEffect> GetRandEffect(int totalLevel)
        {
            // todo
            return null;
        }
    }
}
