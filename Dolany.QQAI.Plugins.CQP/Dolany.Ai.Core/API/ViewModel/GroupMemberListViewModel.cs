﻿using System.Collections.Generic;

namespace Dolany.Ai.Core.API.ViewModel
{
    using Newtonsoft.Json;

    public class GroupMemberListViewModel
    {
        public long[] adm { get; set; }

        public int ec { get; set; }

        public int ext_num { get; set; }

        public int level { get; set; }

        public Dictionary<string, string> levelname { get; set; }

        public int max_admin { get; set; }

        public int max_num { get; set; }

        public int mem_num { get; set; }

        public Dictionary<string, GroupMemberInfoViewModel> members { get; set; }

        public long owner { get; set; }
    }
}
