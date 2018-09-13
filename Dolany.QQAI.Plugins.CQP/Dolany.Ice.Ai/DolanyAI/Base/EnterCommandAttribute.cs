﻿using System;

namespace Dolany.Ice.Ai.DolanyAI
{
    public enum AuthorityLevel
    {
        开发者 = 0,
        群主 = 1,
        管理员 = 2,
        成员 = 3
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class EnterCommandAttribute : Attribute
    {
        public string Command { get; set; }
        public AuthorityLevel AuthorityLevel { get; set; }
        public string Description { get; set; }
        public string Syntax { get; set; }
        public string Tag { get; set; }
        public string SyntaxChecker { get; set; }
        public bool IsDeveloperOnly { get; set; } = false;
        public bool IsPrivateAvailabe { get; set; } = false;
    }
}