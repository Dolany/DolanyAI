﻿using System;

namespace Dolany.Database.Ai
{
    public class SayingSeal : DbBaseEntity
    {
        public long GroupNum { get; set; }
        public long SealMember { get; set; }
        public DateTime CreateTime { get; set; }
        public string Content { get; set; }
    }
}
