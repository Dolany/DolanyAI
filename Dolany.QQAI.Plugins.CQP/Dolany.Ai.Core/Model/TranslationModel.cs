using System;

namespace Dolany.Ai.Core.Model
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class TranslationSendModel
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("word")]
        public string Word { get; set; }
    }

    public class TranslationReceiveModel
    {
        /// <summary>
        /// 标识，要求与请求id相同
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// 目标语言：en,英文;zh,中文
        /// </summary>
        [JsonProperty("aimLang")]
        public string AimLang { get; set; }

        [JsonProperty("meanings")]
        public IEnumerable<TranslationMeaning> Meanings { get; set; }
    }

    public class TranslationMeaning
    {
        [JsonProperty("property")]
        public string Property { get; set; }

        [JsonProperty("details")]
        public IEnumerable<string> Details { get; set; }
    }
}
