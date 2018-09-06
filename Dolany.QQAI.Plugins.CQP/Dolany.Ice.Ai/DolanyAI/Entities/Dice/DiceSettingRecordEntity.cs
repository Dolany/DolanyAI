using System;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class DiceSettingRecordEntity : EntityBase
    {
        [DataColumn]
        public long FromGroup { get; set; }

        [DataColumn]
        public DateTime UpdateTime { get; set; }

        [DataColumn]
        public string SourceFormat { get; set; }
    }
}