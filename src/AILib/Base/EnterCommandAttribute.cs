using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib
{
    public enum AuthorityLevel
    {
        群主 = 1,
        管理员 = 2,
        成员 = 0
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
    public class EnterCommandAttribute : Attribute
    {
        public string Command { get; set; }
        public MsgType SourceType { get; set; }
        public bool IsDeveloperOnly { get; set; }
        public AuthorityLevel AuthorityLevel { get; set; }
        public string Description { get; set; }
        public string Syntax { get; set; }
    }
}