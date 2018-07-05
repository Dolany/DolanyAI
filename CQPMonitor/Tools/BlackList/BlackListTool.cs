using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQPMonitor.Tools.BlackList;

namespace CQPMonitor.Tools
{
    public class BlackListTool : ToolBase
    {
        public BlackListTool()
        {
            Name = "黑名单管理";
            Decription = "管理黑名单以及屏蔽词";
            Icon = "blacklist.ico";
            IsAutoStart = true;
            RelatedForm = new BlackListForm();
        }

        public override void Work()
        {
            base.Work();
        }
    }
}