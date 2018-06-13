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
        public string Creator { get; set; }

        [DataColumn]
        public string Decription { get; set; }

        [DataColumn]
        public string Available { get; set; }
    }
}
