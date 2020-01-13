using System.Collections.Generic;

namespace Dolany.Ai.Core.Base
{
    public abstract class IWorldLine
    {
        public abstract string Name { get; set; }
        public abstract List<AIBase> AIGroup { get; set; }
    }
}
