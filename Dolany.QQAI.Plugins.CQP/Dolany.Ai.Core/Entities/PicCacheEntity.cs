using System;

namespace Dolany.Ai.Core.Entities
{
    public class PicCacheEntity : EntityBase
    {
        [DataColumn]
        public long FromGroup { get; set; }

        [DataColumn]
        public long FromQQ { get; set; }

        [DataColumn]
        public DateTime SendTime { get; set; } = DateTime.Now;
    }
}
