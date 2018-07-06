﻿using System;
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
        public object RelatedObj = null;

        public dolanyToolCon()
        {
            InitializeComponent();
        }

        public dolanyToolCon(string name, string status, string description, string icon, object RelatedObj)
            : this()
        {
            this.RelatedObj = RelatedObj;
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

        private void StatusLbl_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }

        private void DescriptionLbl_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }
    }
}