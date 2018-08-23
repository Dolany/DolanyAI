using System;

namespace Dolany.Ice.Ai.DolanyAI
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