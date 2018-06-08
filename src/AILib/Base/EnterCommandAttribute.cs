using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib
{
    public enum MsgSourceType
    {
        Private,
        Group
    }

    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class EnterCommandAttribute : Attribute
    {
        public string Command { get; set; }
        public MsgSourceType SourceType { get; set; }
    }
}
