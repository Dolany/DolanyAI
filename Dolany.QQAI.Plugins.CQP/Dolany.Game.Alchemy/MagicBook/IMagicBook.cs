using System.Collections.Generic;

namespace Dolany.Game.Alchemy.MagicBook
{
    public abstract class IMagicBook
    {
        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract List<IAlItem> Items { get; }
    }
}
