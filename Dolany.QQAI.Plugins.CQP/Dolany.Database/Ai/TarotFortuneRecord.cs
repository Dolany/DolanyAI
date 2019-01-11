using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Database.Ai
{
    public partial class TarotFortuneRecord
    {
        public string Id { get; set; }
        public long QQNum { get; set; }
        public string TarotId { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
