using System;
using System.Drawing;
using System.Windows.Forms;

namespace DolanyToolControl
{
    public partial class dolanyToolCon : UserControl
    {
        public dolanyToolCon()
        {
            InitializeComponent();
        }

        public dolanyToolCon(string name, string description, string icon)
            : this()
        {
            NameLbl.Text = name;

            var toolTip = new ToolTip
            {
                ShowAlways = true
            };
            toolTip.SetToolTip(panel1, description);

            panel1.BackgroundImage = Image.FromFile(icon);
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }

        private void NameLbl_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }
    }
}