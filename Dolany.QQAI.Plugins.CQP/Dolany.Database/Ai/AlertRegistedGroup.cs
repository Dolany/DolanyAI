using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Database.Ai
{
    public partial class AlertRegistedGroup : BaseEntity
    {
        public string Available { get; set; }
        public long GroupNum { get; set; }
        public long AINum { get; set; }
    }
}
