using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Db
{
    public partial class Saying
    {
        public string Id { get; set; }
        public string Cartoon { get; set; }
        public string Charactor { get; set; }
        public long FromGroup { get; set; }
        public string Content { get; set; }
    }
}
