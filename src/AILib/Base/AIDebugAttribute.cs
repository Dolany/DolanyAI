using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class AIDebugAttribute : Attribute
    {
        public string EntrancePoint { get; set; }
    }
}
