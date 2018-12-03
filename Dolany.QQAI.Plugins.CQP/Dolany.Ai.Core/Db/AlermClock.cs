﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Db
{
    public partial class AlermClock
    {
        public string Id { get; set; }
        public long GroupNumber { get; set; }
        public long Creator { get; set; }
        public System.DateTime CreateTime { get; set; }
        public int AimHourt { get; set; }
        public int AimMinute { get; set; }
        public string Content { get; set; }
        public long AINum { get; set; }
    }
}
