﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQPMonitor.Tools
{
    public class AutoRestartTool : ToolBase
    {
        public AutoRestartTool()
        {
            Name = "自动重启";
            Decription = "自动重启CQP，检测历史重启情况";
            Icon = "autorestart.ico";
            IsAutoStart = true;
        }
    }
}