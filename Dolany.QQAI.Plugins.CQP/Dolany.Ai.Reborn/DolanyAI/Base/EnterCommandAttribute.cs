﻿using System;
using System.Collections.Generic;

namespace Dolany.Ai.Reborn.DolanyAI.Base
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
        public bool IsPrivateAvailabe { get; set; }

        private IEnumerable<string> _CommandsList;

        public IEnumerable<string> CommandsList => _CommandsList ?? (_CommandsList = Command.Split(' '));
    }
}