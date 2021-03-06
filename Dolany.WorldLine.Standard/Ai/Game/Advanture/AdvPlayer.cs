﻿using Dolany.Ai.Common;
using Dolany.Ai.Core.Common;
using Dolany.Database;
using Dolany.UtilityTool;

namespace Dolany.WorldLine.Standard.Ai.Game.Advanture
{
    public class AdvPlayer : DbBaseEntity
    {
        public long QQNum { get; set; }

        public int Exp { get; set; }

        public int HP { get; set; }

        public int Level { get; set; }

        public int MinAtk { get; set; }

        public int MaxAtk { get; set; }

        public int WinTotal { get; set; }

        public int GameTotal { get; set; }

        public string EmojiLevel => Utility.LevelEmoji(Level);

        public static AdvPlayer GetPlayer(long QQNum)
        {
            var player = MongoService<AdvPlayer>.GetOnly(p => p.QQNum == QQNum);
            if (player != null)
            {
                return player;
            }

            player = new AdvPlayer()
            {
                QQNum = QQNum,
                MinAtk = 0,
                MaxAtk = 10,
                Level = 1,
                HP = 10
            };
            MongoService<AdvPlayer>.Insert(player);

            return player;
        }

        public int GetAtk()
        {
            return Rander.RandRange(MinAtk, MaxAtk);
        }

        public void Update()
        {
            MongoService<AdvPlayer>.Update(this);
        }

        public void BattleRecord(bool isWinner)
        {
            GameTotal++;
            if (isWinner)
            {
                WinTotal++;
            }
        }
    }
}
