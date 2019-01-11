using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Database.Ai
{
    public partial class MsgSendCache : BaseEntity
    {
        public long Aim { get; set; }
        public int Type { get; set; }
        public string Msg { get; set; }
        public string Guid { get; set; }
        public int SerialNum { get; set; }
        public long AINum { get; set; }
    }
}
