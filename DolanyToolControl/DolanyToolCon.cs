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
            StatusLbl.Text = status;
            DescriptionLbl.Text = description;
            IconBox.Image = Image.FromFile(icon);
        }
    }
}