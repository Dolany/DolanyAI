using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Database.Ai
{
    public partial class SayingSeal
    {
        public string Id { get; set; }
        public long GroupNum { get; set; }
        public long SealMember { get; set; }
        public DateTime CreateTime { get; set; }
        public string Content { get; set; }
    }
}
