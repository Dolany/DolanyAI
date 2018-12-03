using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Db
{
    public partial class MsgRecievedCache
    {
        public string Id { get; set; }
        public string Msg { get; set; }
        public long FromGroup { get; set; }
        public long FromQQ { get; set; }
        public System.DateTime Time { get; set; }
        public string FullMsg { get; set; }
        public string Command { get; set; }
    }
}
