using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Db
{
    public partial class TarotFortuneData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsPos { get; set; }
        public string Description { get; set; }
        public string PicSrc { get; set; }
    }
}
