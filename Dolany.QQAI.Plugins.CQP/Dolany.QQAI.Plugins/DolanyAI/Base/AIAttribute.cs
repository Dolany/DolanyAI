using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.QQAI.Plugins.DolanyAI
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class AIAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public int PriorityLevel { get; set; }
    }
}