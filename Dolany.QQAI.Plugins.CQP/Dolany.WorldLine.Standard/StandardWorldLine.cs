using System.Collections.Generic;
using System.Reflection;
using Dolany.Ai.Core.Base;

namespace Dolany.WorldLine.Standard
{
    public class StandardWorldLine : IWorldLine
    {
        public override string Name { get; set; } = "Standard";
        public override List<AIBase> AIGroup { get; set; }
    }
}
