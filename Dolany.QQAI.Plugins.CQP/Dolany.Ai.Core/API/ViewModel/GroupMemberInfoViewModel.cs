namespace Dolany.Ai.Core.API.ViewModel
{
    using Newtonsoft.Json;

    public class GroupMemberInfoViewModel
    {
        [JsonProperty("card")]
        public string Card { get; set; }

        [JsonProperty("flag")]
        public int Flag { get; set; }

        [JsonProperty("g")]
        public int G { get; set; }

        [JsonProperty("join_time")]
        public long Join_time { get; set; }

        [JsonProperty("last_speak_time")]
        public long Last_speak_time { get; set; }

        [JsonProperty("lv")]
        public GroupMemberLevelViewModel Lv { get; set; }

        [JsonProperty("nick")]
        public string Nick { get; set; }

        [JsonProperty("qage")]
        public int Qage { get; set; }

        [JsonProperty("role")]
        public int Role { get; set; }

        [JsonProperty("tags")]
        public int Tags { get; set; }

        [JsonProperty("uin")]
        public long Uin { get; set; }
    }
}
