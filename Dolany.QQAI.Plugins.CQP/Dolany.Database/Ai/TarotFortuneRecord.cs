using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Database.Ai
{
    public partial class TarotFortuneRecord : BaseEntity
    {
        public long QQNum { get; set; }
        public string TarotId { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
