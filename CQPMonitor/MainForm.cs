using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CQPMonitor.Tools;
using DolanyToolControl;
using System.IO;
using System.Reflection;
using System.ComponentModel.Composition;

namespace CQPMonitor
{
    public partial class MainForm : Form
    {
        private string ImagePath = "./Image/";

        [ImportMany]
        private IEnumerable<Lazy<ToolBaseForm, IToolCapabilities>> Tools;

        public MainForm()
        {
            InitializeComponent();

            this.ComposePartsSelf(Assembly.GetExecutingAssembly());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadAllTools();
            LayoutTools();
        }

        private void LoadAllTools()
        {
            Tools = Tools.OrderBy(t => t.Metadata.Order).ToList();
        }

        private void LayoutTools()
        {
            foreach (var tool in Tools)
            {
                try
                {
                    LayoutTool(tool.Value);
                }
                catch { }
            }
        }

        private void LayoutTool(ToolBaseForm tool)
        {
            dolanyToolCon dolanyTool = new dolanyToolCon(
                tool.ToolAttr.ToolName,
                "",
                tool.ToolAttr.Decription,
                ImagePath + tool.ToolAttr.ToolIcon
                );

            dolanyTool.Parent = MainPanel;
            dolanyTool.Click += onTool_Click;
            tool.RelatedControl = dolanyTool;
        }

        private void onTool_Click(object sender, EventArgs e)
        {
            if (sender != null || sender is dolanyToolCon)
            {
                var toolCon = sender as dolanyToolCon;
                var tool = Tools.Where(t => t.Value.RelatedControl == toolCon).First();
                tool.Value.ShowTool();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.QPan_OpenFromTuoPan();
        }

        private void QPan_OpenFromTuoPan()
        {
            this.Visible = true;
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Normal;
            this.notifyIcon1.Visible = true;
        }

        private void MenuItemOpen_Click(object sender, EventArgs e)
        {
            this.QPan_OpenFromTuoPan();
        }

        private void MenuItemClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.QPan_MiniMizedToTuoPan();
            }
        }

        private void QPan_MiniMizedToTuoPan()
        {
            this.Hide();
            this.ShowInTaskbar = false;
            this.notifyIcon1.Visible = true;
        }

        private void ppMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("确定要关闭程序吗？", "注意", MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                e.Cancel = true;
            }
        }
    }
}