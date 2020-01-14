namespace Dolany.WorldLine.Standard.Ai.Game.Advanture.Cave
{
    public class MonsterCave : ICave
    {
        public override string Description => $"[怪物]{Name}(攻击力{Atk} 生命值{HP})";
        public override CaveType Type { get; set; } = CaveType.怪兽;
        public override bool IsNeedRefresh => HP <= 0;

        public string Name { get;set; }

        public int HP { get; set; }

        public int Atk { get; set; }
    }
}
