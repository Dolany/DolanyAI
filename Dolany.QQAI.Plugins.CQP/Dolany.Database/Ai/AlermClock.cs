using System;

namespace Dolany.Database.Ai
{
    public class AlermClock : DbBaseEntity
    {
        public long GroupNumber { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public int AimHourt { get; set; }
        public int AimMinute { get; set; }
        public string Content { get; set; }
        public long AINum { get; set; }
    }
}
