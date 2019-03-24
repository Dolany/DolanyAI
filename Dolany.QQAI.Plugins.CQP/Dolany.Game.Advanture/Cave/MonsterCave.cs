using Dolany.Ai.Common;

namespace Dolany.Game.Advanture.Cave
{
    public class MonsterCave : ICave
    {
        public override string Description => $"{Emoji.怪兽}{Name}({Emoji.剑}{Atk} {Emoji.心}{HP})";
        public override CaveType Type { get; set; } = CaveType.怪兽;
        public override bool IsNeedRefresh => HP <= 0;

        public string Name { get;set; }

        public int HP { get; set; }

        public int Atk { get; set; }
    }
}
