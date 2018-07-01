using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class HelloEntity : EntityBase
    {
        [DataColumn]
        public long GroupNum { get; set; }

        [DataColumn]
        public long QQNum { get; set; }

        [DataColumn]
        public string LastHelloDate { get; set; }
    }
}