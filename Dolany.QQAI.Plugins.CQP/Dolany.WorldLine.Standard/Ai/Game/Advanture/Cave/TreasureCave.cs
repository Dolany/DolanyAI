namespace Dolany.WorldLine.Standard.Ai.Game.Advanture.Cave
{
    public class TreasureCave : ICave
    {
        public override string Description => $"[宝箱]{Name}(生命值{HP})";
        public override CaveType Type { get; set; } = CaveType.宝箱;
        public override bool IsNeedRefresh => HP <= 0;

        public string Name { get; set; }

        public int HP { get; set; }

        public int Golds { get; set; }
    }
}
