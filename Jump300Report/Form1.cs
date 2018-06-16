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

namespace Jump300Report
{
    public partial class Form1 : Form
    {
        private JumpReportRequestor JRR;

        public Form1()
        {
            InitializeComponent();
        }

        private void QueryBtn_Click(object sender, EventArgs e)
        {
            string name = nameTxt.Text;
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("ID不能为空！");
                return;
            }

            JRR = new JumpReportRequestor(new GroupMsgDTO()
            {
                msg = "Dolany的宏世界"
            }, ReportCallBack);

            Task.Run(new Action(() => JRR.Work()));
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