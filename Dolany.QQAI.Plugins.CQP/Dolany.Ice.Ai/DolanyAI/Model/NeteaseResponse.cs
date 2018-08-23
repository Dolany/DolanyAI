using System.Collections.Generic;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class NeteaseResponseModel
    {
        public NeteaseResponse_Result result { get; set; }
        public int? code { get; set; }
    }

    public class NeteaseResponse_Result
    {
        public IEnumerable<NeteaseResponse_Song> songs { get; set; }
        public int? songCount { get; set; }
    }

    public class NeteaseResponse_Song
    {
        public string id { get; set; }
        public string name { get; set; }
        public IEnumerable<NeteaseResponse_Artist> artists { get; set; }
        public NeteaseResponse_Album album { get; set; }
        public long? duration { get; set; }
        public string copyrightId { get; set; }
        public int? status { get; set; }
        public IEnumerable<object> alias { get; set; }
        public int? rtype { get; set; }
        public int? ftype { get; set; }
        public int? mvid { get; set; }
        public double? fee { get; set; }
        public string rUrl { get; set; }
    }

    public class NeteaseResponse_Artist
    {
        public string id { get; set; }
        public string name { get; set; }
        public string picUrl { get; set; }
        public IEnumerable<object> alias { get; set; }
        public int? albumSize { get; set; }
        public string picId { get; set; }
        public string img1v1Url { get; set; }
        public int? img1v1 { get; set; }
        public string trans { get; set; }
    }

    public class NeteaseResponse_Album
    {
        public string id { get; set; }
        public string name { get; set; }
        public NeteaseResponse_Artist artist { get; set; }
        public long? publishTime { get; set; }
        public int? size { get; set; }
        public string copyrightId { get; set; }
        public int? status { get; set; }
        public string picId { get; set; }
    }
}