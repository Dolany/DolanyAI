using System;

namespace Dolany.Database.Ai
{
    public class MsgCommand : BaseEntity
    {
        public string Msg { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public string Command { get; set; }
        public long ToQQ { get; set; }
        public long ToGroup { get; set; }
        public long AiNum { get; set; }
    }
}
