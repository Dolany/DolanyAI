using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;

namespace DolanyTimingSvc
{
    public class TimingSvcMgr : IDependency
    {
        private List<IAITool> ToolGroup { get; set; }

        public void Init(IEnumerable<Assembly> assemblies)
        {
            ToolGroup = AutofacSvc.LoadAllInstanceFromInterface<IAITool>(assemblies);
        }

        public void Load()
        {
            ToolGroup = ToolGroup.Where(p => p.Enabled).ToList();
            foreach (var tool in ToolGroup)
            {
                tool.Work();
            }
        }
    }
}
