using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class GroupConfigEntity : EntityBase
    {
        [DataColumn]
        public long Creator { get; set; }

        [DataColumn]
        public string DeDecription { get; set; }

        [DataColumn]
        public string Available { get; set; }
    }
}
