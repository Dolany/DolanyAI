﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Database.Ai
{
    public partial class TouhouCardRecord
    {
        public string Id { get; set; }
        public long QQNum { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CardName { get; set; }
    }
}
