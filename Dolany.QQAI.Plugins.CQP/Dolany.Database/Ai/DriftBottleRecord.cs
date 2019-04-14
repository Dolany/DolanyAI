namespace Dolany.Database.Ai
{
    using System;

    public class DriftBottleRecord : DbBaseEntity
    {
        public long FromGroup { get; set; }

        public long FromQQ { get; set; }

        public long? ReceivedGroup { get; set; }

        public long? ReceivedQQ { get; set; }

        public DateTime SendTime { get; set; }

        public DateTime? ReceivedTime { get; set; }

        public string Content { get; set; }

        public string SignName { get; set; } = "陌生人";
    }
}
