using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class AISealEntity : EntityBase
    {
        [DataColumn]
        public long GroupNum { get; set; }
    }
}