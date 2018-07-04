using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class BlackListEntity : EntityBase
    {
        [DataColumn]
        public DateTime UpdateTime { get; set; }

        [DataColumn]
        public long QQNum { get; set; }

        [DataColumn]
        public int BlackCount { get; set; }
    }
}