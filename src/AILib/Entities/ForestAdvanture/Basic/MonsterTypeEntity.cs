using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class MonsterTypeEntity : EntityBase
    {
        [DataColumn]
        public string Name { get; set; }

        [DataColumn]
        public int HP { get; set; }

        [DataColumn]
        public int MP { get; set; }

        [DataColumn]
        public string MonsterRaceId { get; set; }
    }
}