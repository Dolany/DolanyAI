using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Db
{
    public partial class MsgSendCache
    {
        public string Id { get; set; }
        public long Aim { get; set; }
        public int Type { get; set; }
        public string Msg { get; set; }
        public string Guid { get; set; }
        public int SerialNum { get; set; }
        public long AINum { get; set; }
    }
}
