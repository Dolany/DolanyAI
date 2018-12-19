﻿using System;

namespace Dolany.Ai.Core.Ai.Game.Jump300Report
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class JumpAnalyzeAttribute : Attribute
    {
        public int Order { get; set; }
        public string Title { get; set; }
    }
}