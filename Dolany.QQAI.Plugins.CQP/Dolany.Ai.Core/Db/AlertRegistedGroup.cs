using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Db
{
    public partial class AlertRegistedGroup
    {
        public string Id { get; set; }
        public string Available { get; set; }
        public long GroupNum { get; set; }
        public long AINum { get; set; }
    }
}
