using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Base
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AIAttribute : Attribute
    {
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public string Name { get; set; }
        public int PriorityLevel { get; set; }
    }
}
