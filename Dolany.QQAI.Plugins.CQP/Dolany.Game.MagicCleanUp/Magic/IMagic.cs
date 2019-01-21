using System.Collections.Generic;
using System.Linq;

namespace Dolany.Game.MagicCleanUp.Magic
{
    public abstract class IMagic
    {
        public abstract string Name { get; set; }

        public abstract string Description { get; set; }

        public abstract List<ElementKind> ElementArrange { get; set; }

        public abstract int Level { get; set; }

        public abstract List<Effect> Effects { get; set; }

        public bool Check(List<ElementKind> Command)
        {
            if (ElementArrange.Count != Command.Count)
            {
                return false;
            }

            return !ElementArrange.Where((t, i) => ElementArrange[i] != Command[i]).Any();
        }
    }
}
