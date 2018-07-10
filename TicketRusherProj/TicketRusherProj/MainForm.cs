using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicketRusherProj.Parser;

namespace TicketRusherProj
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            LoginParser parser = new LoginParser();
            parser.Load("http://m.damai.cn");
        }
    }
}