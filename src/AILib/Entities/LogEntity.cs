using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class LogEntity : EntityBase
    {
        [DataColumn]
        public DateTime CreateTime { get; set; }
    }
}
