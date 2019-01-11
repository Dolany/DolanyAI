using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Database.Ai
{
    public partial class AlertContent : BaseEntity
    {
        public long FromGroup { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public int AimHour { get; set; }
        public string Content { get; set; }
        public long AINum { get; set; }
    }
}
