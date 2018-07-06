using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQPMonitor.Tools.AISealMgr;

namespace CQPMonitor.Tools
{
    public class AISealMgrTool : ToolBase
    {
        public AISealMgrTool()
        {
            Name = "AI封禁管理";
            Decription = "AI封禁管理";
            Icon = "aiseal.ico";
            IsAutoStart = true;
            RelatedForm = new AISealMgrForm();
        }

        public override void Work()
        {
            base.Work();
        }
    }
}