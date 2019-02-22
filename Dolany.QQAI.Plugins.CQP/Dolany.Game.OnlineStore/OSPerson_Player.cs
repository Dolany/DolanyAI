using System;
using System.Collections.Generic;

namespace Dolany.Game.OnlineStore
{
    public partial class OSPerson
    {
        public int Level { get; set; }
        public int MaxHP { get; set; }
        public int CurHP { get; set; }
        public int MaxMP { get; set; }
        public int CurMP { get; set; }
        public int MPRestoreRate { get; set; }
        public DateTime MPRestoreTime { get;set; }
        public int MagicDirt { get; set; }
        public Dictionary<string, int> SpellCardDic { get; set; }
        public bool IsAlive => CurHP > 0;

        public int CurrentMP
        {
            get
            {
                UpdateMP();
                return CurMP;
            }
        }

        private void UpdateMP()
        {
            var span = DateTime.Now - MPRestoreTime.ToLocalTime();
            var seconds = (int)span.TotalSeconds;
            var restoreNum = seconds / MPRestoreRate;
            if (restoreNum == 0)
            {
                return;
            }

            MPRestore(restoreNum);
        }

        public void DoDamage(int value)
        {
            CurHP = CurHP < value ? 0 : CurHP - value;
        }

        public void DoHeal(int value)
        {
            CurHP = CurHP + value > MaxHP ? MaxHP : CurHP + value;
        }

        public void MPCost(int value)
        {
            CurMP = CurMP < value ? 0 : CurMP - value;
            MPRestoreTime = DateTime.Now;
        }

        public void MPRestore(int value)
        {
            CurMP = CurMP + value > MaxMP ? MaxMP : CurMP + value;
            MPRestoreTime = DateTime.Now;
        }
    }
}
