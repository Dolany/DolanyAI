using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class FishItemEntity : EntityBase
    {
        [DataColumn]
        public string Name { get; set; }

        [DataColumn]
        public string Description { get; set; }

        [DataColumn]
        public int RareRate { get; set; }
    }
}