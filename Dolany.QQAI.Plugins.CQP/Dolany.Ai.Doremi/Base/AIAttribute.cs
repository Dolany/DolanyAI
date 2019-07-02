﻿using System;

namespace Dolany.Ai.Doremi.Base
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AIAttribute : Attribute
    {
        public string Description { get; set; }
        public bool Enable { get; set; }
        public string Name { get; set; }
        public int PriorityLevel { get; set; }
        public bool NeedManulOpen { get; set; } = false;
        public string BindAi { get; set; }
    }
}