using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.QQAI.Plugins.CQP.DolanyAI
{
    [AttributeUsage(AttributeTargets.Method)]
    public class JumpAnalyzeAttribute : Attribute
    {
        public int Order { get; set; }
        public string Title { get; set; }
    }
}