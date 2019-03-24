using Dolany.Ai.Common;

namespace Dolany.Game.Advanture.Cave
{
    public class TreasureCave : ICave
    {
        public override string Description => $"{Emoji.礼物}{Name}({Emoji.心}{HP})";
        public override CaveType Type { get; set; } = CaveType.宝箱;
        public override bool IsNeedRefresh => HP <= 0;

        public string Name { get; set; }

        public int HP { get; set; }

        public int Golds { get; set; }
    }
}
