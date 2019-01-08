using System.Collections.Generic;

namespace Dolany.Ai.Core.API.ViewModel
{
    using Newtonsoft.Json;

    public class GroupMemberListViewModel
    {
        [JsonProperty("adm_max")]
        public int Adm_max { get; set; }

        [JsonProperty("adm_num")]
        public int Adm_num { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("ec")]
        public int Ec { get; set; }

        [JsonProperty("max_count")]
        public int Max_count { get; set; }

        [JsonProperty("mems")]
        public IEnumerable<GroupMemberInfoViewModel> Mems { get; set; }

        [JsonProperty("search_count")]
        public int Search_count { get; set; }

        [JsonProperty("svr_time")]
        public long Svr_time { get; set; }

        [JsonProperty("vecsize")]
        public int Vecsize { get; set; }
    }
}
