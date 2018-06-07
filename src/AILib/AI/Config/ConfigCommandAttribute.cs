using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.AI.Config
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class ConfigCommandAttribute : Attribute
    {
        public string Command { get; set; }
    }
}
