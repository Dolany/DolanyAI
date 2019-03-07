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
}
