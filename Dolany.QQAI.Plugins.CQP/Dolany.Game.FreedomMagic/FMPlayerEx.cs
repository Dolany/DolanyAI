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
            this.MaxHP = FMLevelHelper.Instance[Level].MaxHP;
            this.MaxMP = FMLevelHelper.Instance[Level].MaxMP;

            this.CurHP = MaxHP;
            this.CurMP = MaxMP;
        }

        public bool ExpGen(int value)
        {
            CurExp += value;
            if (CurExp < FMLevelHelper.Instance[Level].FullExp)
            {
                return false;
            }

            Level += 1;
            CurExp = 0;

            return true;
        }
    }
}
