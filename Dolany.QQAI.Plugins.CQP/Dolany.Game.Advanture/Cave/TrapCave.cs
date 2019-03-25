namespace Dolany.Game.Advanture.Cave
{
    public class TrapCave : ICave
    {
        public override string Description => $"[陷阱]{Name}";
        public override CaveType Type { get; set; } = CaveType.陷阱;

        public string Name { get; set; }
        public int Atk { get; set; }
    }
}
