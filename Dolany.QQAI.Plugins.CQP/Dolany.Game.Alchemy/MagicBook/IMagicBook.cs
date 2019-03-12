using System.Collections.Generic;
using System.Linq;

namespace Dolany.Game.Alchemy.MagicBook
{
    public abstract class IMagicBook
    {
        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract string ObtainCondition { get;}

        public abstract List<IAlItem> Items { get; }

        public override string ToString()
        {
            var msg = $"《{Name}》\r";
            msg += $"{Description}\r";
            msg += $"解锁条件：{ObtainCondition}\r";
            msg += $"包含物品：{string.Join(",", Items.Select(i => i.Name))}";

            return msg;
        }
    }
}
