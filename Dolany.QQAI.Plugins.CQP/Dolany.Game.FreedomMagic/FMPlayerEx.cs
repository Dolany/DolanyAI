namespace Dolany.Game.FreedomMagic
{
    using System.Collections.Generic;

    public class FMPlayerEx : FMPlayer
    {
        public int CurHP { get; set; }

        public int CurMP { get; set; }

        public int MaxHP { get; set; }

        public int MaxMP { get; set; }

        public List<string> MagicLastTurn { get; set; } = new List<string>();
        public List<string> MagicThisTurn { get; set; } = new List<string>();

        public FMPlayerEx(FMPlayer player)
        {
            this.QQNum = player.QQNum;
            this.Magics = player.Magics;
            this.Level = player.Level;
            this.MaxHP = player.MaxHP;
            this.MaxMP = player.MaxMP;

            this.CurHP = MaxHP;
            this.CurMP = MaxMP;
        }

        public string ExpGen(int value)
        {
            // todo
            return null;
        }
    }
}
