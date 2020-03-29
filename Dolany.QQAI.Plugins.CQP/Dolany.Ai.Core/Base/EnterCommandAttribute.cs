using System;
using System.Collections.Generic;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Common;

namespace Dolany.Ai.Core.Base
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class EnterCommandAttribute : Attribute
    {
        public string ID { get; set; }
        public string Command { get; set; }
        public AuthorityLevel AuthorityLevel { get; set; } = AuthorityLevel.成员;
        public string Description { get; set; }
        public string SyntaxHint { get; set; } = "";
        public CmdTagEnum Tag { get; set; } = CmdTagEnum.Default;
        public string SyntaxChecker { get; set; } = "Empty";
        public bool IsPrivateAvailable { get; set; }
        public bool IsGroupAvailable { get; set; } = true;
        public bool IsTesting { get; set; } = false;
        public int DailyLimit { get; set; }
        public int TestingDailyLimit { get; set; }

        private IEnumerable<string> _CommandsList;

        public IEnumerable<string> CommandsList => _CommandsList ??= Command.Split(' ');
    }
}
