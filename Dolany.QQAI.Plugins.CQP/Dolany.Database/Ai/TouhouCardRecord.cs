using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Database.Ai
{
    public partial class TouhouCardRecord : BaseEntity
    {
        public long QQNum { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CardName { get; set; }
    }
}
