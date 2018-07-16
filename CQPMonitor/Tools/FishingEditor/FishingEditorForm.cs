using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AILib.Db;

namespace CQPMonitor.Tools.FishingEditor
{
    [Tool(
        ToolName = "钓鱼编辑器",
        Decription = "钓鱼编辑器",
        ToolIcon = "fishingeditor.ico",
        IsAutoStart = true,
        Order = 4
        )]
    public partial class FishingEditorForm : ToolBaseForm
    {
        public FishingEditorForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }
    }
}