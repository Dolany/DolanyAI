using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Entities
{
    public class SynonymDicEntity : EntityBase
    {
        [DataColumn]
        public string Keyword { get; set; }
    }
}
