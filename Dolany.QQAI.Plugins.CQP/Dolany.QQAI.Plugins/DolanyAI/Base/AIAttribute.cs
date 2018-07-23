using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;

namespace Dolany.QQAI.Plugins.DolanyAI
{
    [MetadataAttribute]
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class AIAttribute : ExportAttribute
    {
        public AIAttribute()
            : base(typeof(AIBase))
        {
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public int PriorityLevel { get; set; }
    }

    public interface IAIExportCapabilities
    {
        string Name { get; }
        string Description { get; }
        bool IsAvailable { get; }
        int PriorityLevel { get; }
    }
}