﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Db
{
    public partial class SayingSeal
    {
        public string Id { get; set; }
        public long GroupNum { get; set; }
        public long SealMember { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string Content { get; set; }
    }
}