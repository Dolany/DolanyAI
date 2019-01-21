using System.Collections.Generic;

namespace Dolany.Game.MagicCleanUp.Magic
{
    public class SimpleHeal : IMagic
    {
        public override string Name { get; set; } = "基础治疗";
        public override string Description { get; set; } = "回复5点生命值";
        public override List<ElementKind> ElementArrange { get; set; } = new List<ElementKind>(){ElementKind.Heal, ElementKind.Heal};
        public override int Level { get; set; } = 1;
        public override List<Effect> Effects { get; set; } = new List<Effect>() {new Effect() {Kind = EffectKind.Heal, Value = 5}};
    }
}
