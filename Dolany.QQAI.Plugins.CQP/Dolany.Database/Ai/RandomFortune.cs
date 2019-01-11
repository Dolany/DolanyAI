using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Database.Ai
{
    public partial class RandomFortune
    {
        public string Id { get; set; }
        public long QQNum { get; set; }
        public DateTime UpdateDate { get; set; }
        public int FortuneValue { get; set; }
        public string BlessName { get; set; }
        public int BlessValue { get; set; }
    }
}
