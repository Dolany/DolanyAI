namespace Dolany.Ai.Core.Model.Tuling
{
    using Newtonsoft.Json;

    public class TulingRequestData
    {
        [JsonProperty("reqType")]
        public int ReqType { get; set; }

        [JsonProperty("perception")]
        public perceptionData Perception { get; set; }

        [JsonProperty("userInfo")]
        public userInfoData UserInfo { get; set; }
    }

    public class perceptionData
    {
        [JsonProperty("inputText")]
        public inputTextData InputText { get; set; }

        [JsonProperty("inputImage")]
        public inputImageData InputImage { get; set; }

        [JsonProperty("inputMedia")]
        public inputMediaData InputMedia { get; set; }

        [JsonProperty("selfInfo")]
        public selfInfoData SelfInfo { get; set; }
    }

    public class inputTextData
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class inputImageData
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class inputMediaData
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class selfInfoData
    {
        [JsonProperty("location")]
        public locationData Location { get; set; }
    }

    public class locationData
    {
        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("province")]
        public string Province { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }
    }

    public class userInfoData
    {
        [JsonProperty("apiKey")]
        public string ApiKey { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }
    }
}
