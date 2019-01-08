namespace Dolany.Ai.Core.API.ViewModel
{
    using Newtonsoft.Json;

    public class GroupMemberLevelViewModel
    {
        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("point")]
        public int Point { get; set; }
    }
}
