using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class AIAttribute : Attribute
    {
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public string Name { get; set; }
        public int PriorityLevel { get; set; }
    }
}