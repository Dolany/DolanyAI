using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dolany.Ice.Ai.DolanyAI;

namespace KanColeVoiceClimber
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => Work());
        }

        private void Work()
        {
            HttpRequester requester = new HttpRequester();

            string aimStr = $"https://zh.moegirl.org/%E8%88%B0%E9%98%9FCollection/%E5%9B%BE%E9%89%B4/%E8%88%B0%E5%A8%98";
            string HtmlStr = requester.Request(aimStr);

            ColeGirlPageParse parser = new ColeGirlPageParse();
            parser.Load(HtmlStr);

            var list = parser.GirlList;
        }
    }
}