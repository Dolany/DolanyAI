using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Entities
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
