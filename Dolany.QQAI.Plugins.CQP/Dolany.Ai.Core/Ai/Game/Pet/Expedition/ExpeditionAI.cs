using Dolany.Ai.Core.Base;

namespace Dolany.Ai.Core.Ai.Game.Pet.Expedition
{
    public class ExpeditionAI : AIBase
    {
        public override string AIName { get; set; } = "宠物远征";
        public override string Description { get; set; } = "AI for Pet Expedition.";
        public override int PriorityLevel { get; set; } = 10;
    }
}
