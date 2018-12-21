using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Game.TouhouCardWar.Db
{
    public class PlayerInfo
    {
        public string Id { get; set; }
        public long QQNum { get; set; }
        public DateTime CreateTime { get; set; }
        public int Score { get; set; }
        public int Level { get; set; }
    }
}
