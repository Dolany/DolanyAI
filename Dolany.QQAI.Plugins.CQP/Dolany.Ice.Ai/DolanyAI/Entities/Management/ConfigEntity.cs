﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class ConfigEntity : EntityBase
    {
        [DataColumn]
        public string Name { get; set; }
    }
}