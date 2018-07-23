using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.QQAI.Plugins.DolanyAI
{
    public enum AuthorityLevel
    {
        群主 = 1,
        管理员 = 2,
        成员 = 0
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
    public class GroupEnterCommandAttribute : Attribute
    {
        public string Command { get; set; }
        public AuthorityLevel AuthorityLevel { get; set; }
        public string Description { get; set; }
        public string Syntax { get; set; }
        public string Tag { get; set; }
        public string SyntaxChecker { get; set; }
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
    public class PrivateEnterCommandAttribute : Attribute
    {
        public string Command { get; set; }
        public bool IsDeveloperOnly { get; set; }
        public string Description { get; set; }
        public string Syntax { get; set; }
        public string Tag { get; set; }
        public string SyntaxChecker { get; set; }
    }
}