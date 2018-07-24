using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.QQAI.Plugins.Snow.DolanyAI
{
    public enum MsgType
    {
        Private = 1,
        Group = 0
    }

    public class SendMsgDTO
    {
        public string Msg { get; set; }
        public long Aim { get; set; }
        public MsgType Type { get; set; }
    }
}