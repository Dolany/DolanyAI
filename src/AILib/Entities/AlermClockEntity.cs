using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class AlermClockEntity : EntityBase
    {
        [PrimaryKey]
        public string Id { get; set; }
        public long GroupNumber { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public int AimHourt { get; set; }
        public int AimMinute { get; set; }
        [Content]
        public string Content { get; set; }
    }
}
