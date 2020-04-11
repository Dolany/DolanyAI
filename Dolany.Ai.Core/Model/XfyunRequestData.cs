namespace Dolany.Ai.Core.Model
{
    using Newtonsoft.Json;

    public class XfyunRequestData
    {
        [JsonProperty("auf")]
        public string Auf { get; set; }

        [JsonProperty("aue")]
        public string Aue { get; set; }

        [JsonProperty("voice_name")]
        public string Voice_name { get; set; }
    }
}
