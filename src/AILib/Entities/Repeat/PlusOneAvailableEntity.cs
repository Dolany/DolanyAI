﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class PlusOneAvailableEntity : EntityBase
    {
        [DataColumn]
        public long GroupNumber { get; set; }
    }
}