using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class EnterCommandAttribute : Attribute
    {
        public string Command { get; set; }
        public MsgType SourceType { get; set; }
    }
}
