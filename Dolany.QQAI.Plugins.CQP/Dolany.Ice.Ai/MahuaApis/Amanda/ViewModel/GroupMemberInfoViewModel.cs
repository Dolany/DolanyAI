using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.MahuaApis
{
    public class GroupMemberInfoViewModel
    {
        public string card { get; set; }
        public int flag { get; set; }
        public int g { get; set; }
        public long join_time { get; set; }
        public long last_speak_time { get; set; }
        public GroupMemberLevelViewModel lv { get; set; }
        public string nick { get; set; }
        public int qage { get; set; }
        public int role { get; set; }
        public int tags { get; set; }
        public long uin { get; set; }
    }
}