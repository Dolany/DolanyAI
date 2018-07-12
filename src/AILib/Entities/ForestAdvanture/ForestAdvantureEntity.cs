using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class ForestAdvantureEntity : EntityBase
    {
        [DataColumn]
        public long GroupNum { get; set; }

        [DataColumn]
        public long QQNum { get; set; }

        [DataColumn]
        public string HeroId { get; set; }

        [DataColumn]
        public string Status { get; set; }

        [DataColumn]
        public DateTime CreateTime { get; set; }
    }
}