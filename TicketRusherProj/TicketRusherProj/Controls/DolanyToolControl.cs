using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicketRusherProj.Controls
{
    public partial class DolanyToolControl : UserControl
    {
        public DolanyToolControl()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        public DolanyToolControl(string name, string status, string description, string icon)
            : this()
        {
            NameLbl.Text = name;

            ToolTip toolTip = new ToolTip();
            toolTip.ShowAlways = true;
            toolTip.SetToolTip(panel1, description);

            try
            {
                panel1.BackgroundImage = Image.FromFile(icon);
            }
            catch
            {
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }

        private void NameLbl_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }
    }
}