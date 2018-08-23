using System;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class JumpAnalyzeAttribute : Attribute
    {
        public int Order { get; set; }
        public string Title { get; set; }
    }
}