using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Database.Ai
{
    public partial class RepeaterAvailable : BaseEntity
    {
        public long GroupNumber { get; set; }
        public bool Available { get; set; }
    }
}
