using System.Collections.Generic;

namespace Dolany.Ai.Core.Model
{
    using Newtonsoft.Json;

    public class NeteaseResponseModel
    {
        [JsonProperty("result")]
        public NeteaseResponse_Result Result { get; set; }

        [JsonProperty("code")]
        public int? Code { get; set; }
    }

    public class NeteaseResponse_Result
    {
        [JsonProperty("songs")]
        public IEnumerable<NeteaseResponse_Song> Songs { get; set; }

        [JsonProperty("songCount")]
        public int? SongCount { get; set; }
    }

    public class NeteaseResponse_Song
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("artists")]
        public IEnumerable<NeteaseResponse_Artist> Artists { get; set; }

        [JsonProperty("album")]
        public NeteaseResponse_Album Album { get; set; }

        [JsonProperty("duration")]
        public long? Duration { get; set; }

        [JsonProperty("copyrightId")]
        public string CopyrightId { get; set; }

        [JsonProperty("status")]
        public int? Status { get; set; }

        [JsonProperty("alias")]
        public IEnumerable<object> Alias { get; set; }

        [JsonProperty("rtype")]
        public int? Rtype { get; set; }

        [JsonProperty("ftype")]
        public int? Ftype { get; set; }

        [JsonProperty("mvid")]
        public int? Mvid { get; set; }

        [JsonProperty("fee")]
        public double? Fee { get; set; }

        [JsonProperty("rUrl")]
        public string RUrl { get; set; }
    }

    public class NeteaseResponse_Artist
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("picUrl")]
        public string PicUrl { get; set; }

        [JsonProperty("alias")]
        public IEnumerable<object> Alias { get; set; }

        [JsonProperty("albumSize")]
        public int? AlbumSize { get; set; }

        [JsonProperty("picId")]
        public string PicId { get; set; }

        [JsonProperty("img1v1Url")]
        public string Img1v1Url { get; set; }

        [JsonProperty("img1v1")]
        public int? Img1v1 { get; set; }

        [JsonProperty("trans")]
        public string Trans { get; set; }
    }

    public class NeteaseResponse_Album
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("artist")]
        public NeteaseResponse_Artist Artist { get; set; }

        [JsonProperty("publishTime")]
        public long? PublishTime { get; set; }

        [JsonProperty("size")]
        public int? Size { get; set; }

        [JsonProperty("copyrightId")]
        public string CopyrightId { get; set; }

        [JsonProperty("status")]
        public int? Status { get; set; }

        [JsonProperty("picId")]
        public string PicId { get; set; }
    }
}
