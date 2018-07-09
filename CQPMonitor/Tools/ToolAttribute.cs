using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQPMonitor.Tools
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ToolAttribute : Attribute
    {
        public string ToolName { get; set; }
        public string Decription { get; set; }
        public string ToolIcon { get; set; }
        public bool IsAutoStart { get; set; }
        public int Order { get; set; }
    }
}