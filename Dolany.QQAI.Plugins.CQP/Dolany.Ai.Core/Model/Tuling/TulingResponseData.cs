using System.Collections.Generic;

namespace Dolany.Ai.Core.Model.Tuling
{
    using Newtonsoft.Json;

    public class TulingResponseData
    {
        [JsonProperty("intent")]
        public TulingResponseData_Intent Intent { get; set; }

        [JsonProperty("results")]
        public IEnumerable<TulingResponseData_Result> Results { get; set; }
    }

    public class TulingResponseData_Intent
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("intentName")]
        public string IntentName { get; set; }

        [JsonProperty("actionName")]
        public string ActionName { get; set; }

        [JsonProperty("parameters")]
        public IEnumerable<KeyValuePair<string, string>> Parameters { get; set; }
    }

    public class TulingResponseData_Result
    {
        [JsonProperty("resultType")]
        public string ResultType { get; set; }

        [JsonProperty("values")]
        public TulingResponseData_Value Values { get; set; }

        [JsonProperty("groupType")]
        public int GroupType { get; set; }
    }

    public class TulingResponseData_Value
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("voice")]
        public string Voice { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("video")]
        public string Video { get; set; }
    }
}
