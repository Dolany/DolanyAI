﻿namespace Dolany.Ai.Core.Base
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AIAttribute : Attribute
    {
        public string Description { get; set; }
        public bool Enable { get; set; }
        public string Name { get; set; }
        public int PriorityLevel { get; set; }
    }
}
