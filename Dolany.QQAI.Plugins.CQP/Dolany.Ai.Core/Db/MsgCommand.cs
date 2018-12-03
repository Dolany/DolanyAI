using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Db
{
    public partial class MsgCommand
    {
        public string Id { get; set; }
        public string Msg { get; set; }
        public System.DateTime Time { get; set; }
        public string Command { get; set; }
        public long ToQQ { get; set; }
        public long ToGroup { get; set; }
    }
}
