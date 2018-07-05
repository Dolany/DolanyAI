using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQPMonitor.Tools
{
    public class ToolBase
    {
        public string Name { get; set; }
        public string Decription { get; set; }
        public string Icon { get; set; }
        public bool IsAutoStart { get; set; }

        public virtual void Work()
        {
        }
    }
}