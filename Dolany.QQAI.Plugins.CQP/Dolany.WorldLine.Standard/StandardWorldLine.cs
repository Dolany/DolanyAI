using Dolany.Ai.Core.Base;

namespace Dolany.WorldLine.Standard
{
    public class StandardWorldLine : IWorldLine
    {
        public override string Name { get; set; } = "经典";
        public override bool IsDefault { get; } = true;
    }
}
