using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Database.Ai
{
    public partial class MsgRecievedCache : BaseEntity
    {
        public string Msg { get; set; }
        public long FromGroup { get; set; }
        public long FromQQ { get; set; }
        public DateTime Time { get; set; }
        public string FullMsg { get; set; }
        public string Command { get; set; }
    }
}
