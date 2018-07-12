using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class CareerEntity : EntityBase
    {
        [DataColumn]
        public string Name { get; set; }

        [DataColumn]
        public string Description { get; set; }

        [DataColumn]
        public string Icon { get; set; }
    }
}