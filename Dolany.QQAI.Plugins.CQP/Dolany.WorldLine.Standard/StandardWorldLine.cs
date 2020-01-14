using System.Collections.Generic;
using Dolany.Ai.Core.Base;

namespace Dolany.WorldLine.Standard
{
    public class StandardWorldLine : IWorldLine
    {
        public override string Name { get; set; } = "Standard";
        protected override List<AIBase> AIGroup { get; set; }
        protected override List<IAITool> ToolGroup { get; set; }
        public override bool IsDefault { get; } = true;
    }
}
