using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Database.Ai
{
    public partial class AIConfig : BaseEntity
    {
        public string Group { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
