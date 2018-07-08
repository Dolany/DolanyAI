using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace CQPMonitor.Tools.AISealMgr
{
    [Tool(
        ToolName = "AI封禁管理",
        Decription = "AI封禁管理",
        ToolIcon = "aiseal.ico",
        IsAutoStart = false,
        Order = 4
        )]
    public partial class AISealMgrForm : ToolBaseForm
    {
        public AISealMgrForm()
        {
            InitializeComponent();
        }
    }
}