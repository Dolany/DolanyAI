using System.Linq;
using System.Windows.Forms;
using DolanyToolControl;

namespace AIMonitor.Tools
{
    public class ToolBaseForm : Form
    {
        private string ImagePath = "./Image/";

        public ToolAttribute ToolAttr
        {
            get
            {
                var t = GetType();
                return t.GetCustomAttributes(typeof(ToolAttribute), false).First() as ToolAttribute;
            }
        }

        public dolanyToolCon RelatedControl { get; set; }

        public ToolBaseForm()
        {
            FormClosing += Form_FormClosing;
        }

        public virtual void Work()
        {
        }

        public void ShowTool()
        {
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            MinimizeBox = false;
            {
                Icon = new System.Drawing.Icon(ImagePath + ToolAttr.ToolIcon);
            }
            Show();
        }

        protected void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        public virtual void Shutdown()
        {
        }
    }
}