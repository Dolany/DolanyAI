using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Database.Ai
{
    public partial class TarotFortuneData : BaseEntity
    {
        public string Name { get; set; }
        public bool IsPos { get; set; }
        public string Description { get; set; }
        public string PicSrc { get; set; }
    }
}
