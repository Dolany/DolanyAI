using System.Collections.Generic;

namespace Dolany.Ai.Core.Model.Tuling
{
    using Newtonsoft.Json;

    public class TulingImportRequestData
    {
        [JsonProperty("apikey")]
        public string ApiKey { get; set; }

        [JsonProperty("data")]
        public TulingImportRequestDataData Data { get; set; }
    }

    public class TulingImportRequestDataData
    {
        [JsonProperty("list")]
        public IEnumerable<TulingImportRequestDataQA> List;
    }

    public class TulingImportRequestDataQA
    {
        [JsonProperty("question")]
        public string Question { get; set; }

        [JsonProperty("answer")]
        public string Answer { get; set; }
    }
}
