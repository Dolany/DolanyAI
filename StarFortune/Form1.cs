using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AILib;

namespace StarFortune
{
    public partial class Form1 : Form
    {
        private FortuneRequestor Requestor;

        public Form1()
        {
            InitializeComponent();
        }

        private void queryBtn_Click(object sender, EventArgs e)
        {
            string name = starCombo.Text;

            Requestor = new FortuneRequestor(new GroupMsgDTO()
            {
                msg = name
            }, ReportCallBack);

            Task.Run(new Action(() => Requestor.Work()));
        }

        private void ReportCallBack(GroupMsgDTO MsgDTO, string Report)
        {
            Invoke(new Action(() =>
            {
                showTxt.Clear();
                showTxt.AppendText(Report);
            }));
        }
    }
}