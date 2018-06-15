using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class SayingSealEntity : EntityBase
    {
        [DataColumn]
        public long GroupNum { get; set; }

        [DataColumn]
        public long SealMember { get; set; }

        [DataColumn]
        public DateTime CreateTime { get; set; }
    }
}
