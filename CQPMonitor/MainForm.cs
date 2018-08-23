using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DolanyToolControl;
using System.Reflection;
using System.ComponentModel.Composition;
using AIMonitor.Tools;

namespace AIMonitor
{
    public partial class MainForm : Form
    {
        private readonly string ImagePath = "./Image/";

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
                if (!tool.Metadata.IsAutoStart)
                {
                    continue;
                }

                LayoutTool(tool.Value);
            }
        }

        private void LayoutTool(ToolBaseForm tool)
        {
            var dolanyTool = new dolanyToolCon(
       tool.ToolAttr.ToolName,
       tool.ToolAttr.Decription,
       ImagePath + tool.ToolAttr.ToolIcon
       )
            {
                Parent = MainPanel
            };
            dolanyTool.Click += onTool_Click;
            tool.RelatedControl = dolanyTool;
        }

        private void onTool_Click(object sender, EventArgs e)
        {
            if (sender != null)
            {
                var toolCon = sender as dolanyToolCon;
                var tool = Tools.First(t => t.Value.RelatedControl == toolCon);
                tool.Value.ShowTool();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            QPan_OpenFromTuoPan();
        }

        private void QPan_OpenFromTuoPan()
        {
            Visible = true;
            Show();
            ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = true;
        }

        private void MenuItemOpen_Click(object sender, EventArgs e)
        {
            QPan_OpenFromTuoPan();
        }

        private void MenuItemClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                QPan_MiniMizedToTuoPan();
            }
        }

        private void QPan_MiniMizedToTuoPan()
        {
            Hide();
            ShowInTaskbar = false;
            notifyIcon1.Visible = true;
        }

        private void ppMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(@"确定要关闭程序吗？", @"注意", MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                e.Cancel = true;
            }
        }
    }
}