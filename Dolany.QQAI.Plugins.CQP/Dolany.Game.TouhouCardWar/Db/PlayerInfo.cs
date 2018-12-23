namespace Dolany.Game.TouhouCardWar.Db
{
    using System;

    public class PlayerInfo
    {
        public string Id { get; set; }
        public long QQNum { get; set; }
        public DateTime CreateTime { get; set; }
        public int Score { get; set; }
        public int Level { get; set; }
    }
}
