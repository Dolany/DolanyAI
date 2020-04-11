using System.Collections.Generic;

namespace Dolany.Ai.Core.Model.Tuling
{
    using Newtonsoft.Json;

    public class TulingImportResponseData
    {
        [JsonProperty("data")]
        public TulingImportResponseDataData Data { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }
    }

    public class TulingImportResponseDataData
    {
        [JsonProperty("successNum")]
        public int SuccessNum { get; set; }

        [JsonProperty("knowledgeList")]
        public IEnumerable<TulingImportResponseDataKnowledge> KnowledgeList { get; set; }
    }

    public class TulingImportResponseDataKnowledge
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("time")]
        public string Time { get; set; }

        [JsonProperty("question")]
        public string Question { get; set; }

        [JsonProperty("label_id")]
        public string Label_id { get; set; }

        [JsonProperty("answer")]
        public string Answer { get; set; }
    }
}
