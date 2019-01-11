using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Database.Ai
{
    public partial class FortuneItem : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Value { get; set; }
        public int Type { get; set; }
    }
}
