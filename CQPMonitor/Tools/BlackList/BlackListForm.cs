using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CQPMonitor.Tools.BlackList
{
    [Tool(
        ToolName = "黑名单管理",
        Decription = "管理黑名单以及屏蔽词",
        ToolIcon = "blacklist.ico",
        IsAutoStart = false,
        Order = 3
        )]
    public partial class BlackListForm : ToolBaseForm
    {
        public BlackListForm()
        {
            InitializeComponent();
        }

        private void RefreshBlackListTable()
        {
        }

        private void RefreshDirtyWordsTable()
        {
        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            RefreshBlackListTable();
        }

        private void BlackListForm_Load(object sender, EventArgs e)
        {
            RefreshBlackListTable();
            RefreshDirtyWordsTable();
        }
    }
}