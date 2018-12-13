using System;

namespace Dolany.Ai.Core.Db
{
    public class MsgCommand
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Msg { get; set; }
        public System.DateTime Time { get; set; } = DateTime.Now;
        public string Command { get; set; }
        public long ToQQ { get; set; }
        public long ToGroup { get; set; }
    }
}
