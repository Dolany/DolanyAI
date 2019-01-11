using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Database.Ai
{
    public partial class Saying : BaseEntity
    {
        public string Cartoon { get; set; }
        public string Charactor { get; set; }
        public long FromGroup { get; set; }
        public string Content { get; set; }
    }
}
