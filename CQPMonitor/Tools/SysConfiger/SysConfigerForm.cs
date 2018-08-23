using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dolany.Ice.Ai.DolanyAI.Db;

namespace AIMonitor.Tools.SysConfiger
{
    [Tool(
        ToolName = "系统配置",
        Decription = "系统配置",
        ToolIcon = "SysConfigerForm.ico",
        IsAutoStart = true,
        Order = 7
        )]
    public partial class SysConfigerForm : ToolBaseForm
    {
        private List<AIConfig> configs = new List<AIConfig>();

        public SysConfigerForm()
        {
            InitializeComponent();
        }

        private void SysConfigerForm_Load(object sender, EventArgs e)
        {
            RefreshConfigs();
        }

        private void RefreshConfigs()
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.AIConfig;
                configs = query.ToList();

                configTable.DataSource = configs;
            }
        }

        private void configTable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (configs.IsNullOrEmpty() || e.RowIndex < 0)
            {
                return;
            }

            using (var cf = new ConfigForm(configs[e.RowIndex]))
            {
                cf.ShowDialog();
                RefreshConfigs();
            }
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            using (var cf = new ConfigForm(null))
            {
                cf.ShowDialog();
                RefreshConfigs();
            }
        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            RefreshConfigs();
        }
    }
}