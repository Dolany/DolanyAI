using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class SynonymDicEntity : EntityBase
    {
        [DataColumn]
        public string Keyword { get; set; }
    }
}
