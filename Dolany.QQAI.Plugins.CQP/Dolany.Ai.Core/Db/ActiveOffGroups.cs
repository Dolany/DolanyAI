using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Db
{
    public partial class ActiveOffGroups
    {
        public string Id { get; set; }
        public long GroupNum { get; set; }
        public System.DateTime UpdateTime { get; set; }
        public long AINum { get; set; }
    }
}
