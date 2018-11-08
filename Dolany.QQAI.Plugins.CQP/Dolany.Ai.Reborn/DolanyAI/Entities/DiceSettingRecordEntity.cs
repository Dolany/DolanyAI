using System;

namespace Dolany.Ai.Reborn.DolanyAI.Entities
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
