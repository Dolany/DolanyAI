using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Database.Ai
{
    public partial class Hello
    {
        public string Id { get; set; }
        public long GroupNum { get; set; }
        public long QQNum { get; set; }
        public DateTime LastHelloDate { get; set; }
        public string Content { get; set; }
    }
}
