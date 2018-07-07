using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DolanyToolControl;

namespace CQPMonitor.Tools
{
    public class ToolBaseForm : Form
    {
        private string ImagePath = "./Image/";

        public string ToolName { get; set; }
        public string Decription { get; set; }
        public string ToolIcon { get; set; }
        public bool IsAutoStart { get; set; }
        public int Order { get; set; }
        public dolanyToolCon RelatedControl { get; set; }

        public ToolBaseForm()
            : base()
        {
            FormClosing += Form_FormClosing;
            StartPosition = FormStartPosition.CenterParent;
        }

        public virtual void Work()
        {
        }

        public void ShowTool()
        {
            try
            {
                Icon = new System.Drawing.Icon(ImagePath + ToolIcon);
            }
            catch { }
            Show();
        }

        protected void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }
    }
}