﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Db
{
    public partial class RandomFortune
    {
        public string Id { get; set; }
        public long QQNum { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public int FortuneValue { get; set; }
        public string BlessName { get; set; }
        public int BlessValue { get; set; }
    }
}
