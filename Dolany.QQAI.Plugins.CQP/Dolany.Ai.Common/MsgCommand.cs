using System;

namespace Dolany.Ai.Common
{
    public class MsgCommand
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Msg { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public CommandType Command { get; set; }
        public long ToQQ { get; set; }
        public long ToGroup { get; set; }
        public string BindAi { get; set; }
    }
}
