using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib
{
    public class GroupMsgDTO
    {
        public int subType { get; set; }

        public int sendTime { get; set; }

        public long fromGroup { get; set; }

        public long fromQQ { get; set; }

        public string fromAnonymous { get; set; }

        public string msg { get; set; }

        public int font { get; set; }

        public string command { get; set; }
    }
}
