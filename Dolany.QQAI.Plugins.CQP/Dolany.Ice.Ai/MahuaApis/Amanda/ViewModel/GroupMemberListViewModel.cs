using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.MahuaApis
{
    public class GroupMemberListViewModel
    {
        public int adm_max { get; set; }
        public int adm_num { get; set; }
        public int count { get; set; }
        public int ec { get; set; }
        public int max_count { get; set; }
        public IEnumerable<GroupMemberInfoViewModel> mems { get; set; }
        public int search_count { get; set; }
        public long svr_time { get; set; }
        public int vecsize { get; set; }
    }
}