using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Db
{
    public partial class TarotFortuneRecord
    {
        public string Id { get; set; }
        public long QQNum { get; set; }
        public string TarotId { get; set; }
        public System.DateTime UpdateTime { get; set; }
    }
}
