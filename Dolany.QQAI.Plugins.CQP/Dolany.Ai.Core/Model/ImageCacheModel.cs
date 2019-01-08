namespace Dolany.Ai.Core.Model
{
    using Newtonsoft.Json;

    public class ImageCacheModel
    {
        [JsonProperty("Guid")]
        public string Guid { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }
    }
}
