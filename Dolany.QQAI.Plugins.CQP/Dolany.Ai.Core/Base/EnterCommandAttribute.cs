using System;
using System.Collections.Generic;

namespace Dolany.Ai.Core.Base
{
    public enum AuthorityLevel
    {
        未知 = 0,
        开发者 = 99,
        群主 = 1,
        管理员 = 2,
        成员 = 3
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class EnterCommandAttribute : Attribute
    {
        public string ID { get; set; }
        public string Command { get; set; }
        public AuthorityLevel AuthorityLevel { get; set; } = AuthorityLevel.未知;
        public string Description { get; set; }
        public string Syntax { get; set; }
        public string Tag { get; set; }
        public string SyntaxChecker { get; set; }
        public bool IsPrivateAvailable { get; set; }
        public bool IsGroupAvailable { get; set; } = true;
        public bool IsTesting { get; set; } = false;
        public int DailyLimit { get; set; }
        public int TestingDailyLimit { get; set; }

        private IEnumerable<string> _CommandsList;

        public IEnumerable<string> CommandsList => _CommandsList ?? (_CommandsList = Command.Split(' '));
    }
}
