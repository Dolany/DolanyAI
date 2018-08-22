using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DolanyToolControl
{
    public partial class dolanyToolCon : UserControl
    {
        public dolanyToolCon()
        {
            InitializeComponent();
        }

        public dolanyToolCon(string name, string status, string description, string icon)
            : this()
        {
            NameLbl.Text = name;

            var toolTip = new ToolTip
            {
                ShowAlways = true
            };
            toolTip.SetToolTip(panel1, description);

            try
            {
                panel1.BackgroundImage = Image.FromFile(icon);
            }
            catch (Exception)
            {
                throw;
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