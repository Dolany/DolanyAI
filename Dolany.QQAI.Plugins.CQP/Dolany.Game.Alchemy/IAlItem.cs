using System.Collections.Generic;

namespace Dolany.Game.Alchemy
{
    public abstract class IAlItem
    {
        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract AlCombineNeed CombineNeed { get; }

        public abstract int BaseSuccessRate { get; }

        public virtual int MaxLevel { get; } = 5;

        public abstract void DeEffect(AlPlayer source, AlPlayer aim, int level);
    }

    public class AlCombineNeed
    {
        public Dictionary<string, int> AlItemNeed { get; set; } = new Dictionary<string, int>();

        public Dictionary<string, int> MagicDirtNeed { get; set; } = new Dictionary<string, int>();

        public Dictionary<string, int> NormalItemNeed { get; set; } = new Dictionary<string, int>();
    }
}
