using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Database.Ai
{
    public partial class BlackList
    {
        public string Id { get; set; }
        public System.DateTime UpdateTime { get; set; }
        public long QQNum { get; set; }
        public int BlackCount { get; set; }
        public string NickName { get; set; }
    }
}
