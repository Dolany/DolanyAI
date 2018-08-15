using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AmandaSharp
{
    public partial class SettingForm : Form
    {
        public  Profiles profiles;
        public SettingForm(Profiles profiles)
        {
            this.profiles = profiles;
            InitializeComponent();
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            profiles.set("hello", textBox1.Text);
            profiles.save();
            this.Dispose();
        }
    }
}
