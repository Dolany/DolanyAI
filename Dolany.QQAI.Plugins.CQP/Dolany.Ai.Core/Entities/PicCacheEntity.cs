using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Entities
{
    public class PicCacheEntity : EntityBase
    {
        [DataColumn]
        public long FromGroup { get; set; }

        [DataColumn]
        public long FromQQ { get; set; }

        [DataColumn]
        public DateTime SendTime { get; set; }
    }
}
