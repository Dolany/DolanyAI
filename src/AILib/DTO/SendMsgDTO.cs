using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib
{
    public enum MsgType
    {
        Private,
        Group
    }

    public class SendMsgDTO
    {
        public string Msg { get; set; }
        public long Aim { get; set; }
        public MsgType Type { get; set; }
    }
}
