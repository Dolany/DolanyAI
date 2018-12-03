using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Db
{
    public partial class TouhouCardRecord
    {
        public string Id { get; set; }
        public long QQNum { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string CardName { get; set; }
    }
}
