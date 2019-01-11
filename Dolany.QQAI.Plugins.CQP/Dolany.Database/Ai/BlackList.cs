using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Database.Ai
{
    public partial class BlackList : BaseEntity
    {
        public DateTime UpdateTime { get; set; }
        public long QQNum { get; set; }
        public int BlackCount { get; set; }
        public string NickName { get; set; }
    }
}
