using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class MonsterRaceEntity : EntityBase
    {
        [DataColumn]
        public string Name { get; set; }
    }
}