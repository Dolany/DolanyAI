using System;

namespace Dolany.Database.Ai
{
    public class Hello : DbBaseEntity
    {
        public long GroupNum { get; set; }
        public long QQNum { get; set; }
        public DateTime LastHelloDate { get; set; }
        public string Content { get; set; }
    }
}
