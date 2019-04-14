using System;

namespace Dolany.Database.Ai
{
    public class BlackList : DbBaseEntity
    {
        public DateTime UpdateTime { get; set; }
        public long QQNum { get; set; }
        public int BlackCount { get; set; }
        public string NickName { get; set; }
    }
}
