using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Database.Ai
{
    public partial class PraiseRec : BaseEntity
    {
        public long QQNum { get; set; }
        public DateTime LastDate { get; set; }
    }
}
