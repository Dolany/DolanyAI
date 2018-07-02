using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class RandomFortuneEntity : EntityBase
    {
        [DataColumn]
        public long QQNum { get; set; }

        [DataColumn]
        public string UpdateDate { get; set; }
    }
}