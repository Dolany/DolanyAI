using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class AlertRegistedGroupEntity : EntityBase
    {
        [DataColumn]
        public string Available { get; set; }
    }
}
