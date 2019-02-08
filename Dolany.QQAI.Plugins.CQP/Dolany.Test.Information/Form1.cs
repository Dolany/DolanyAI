using System;
using System.Windows.Forms;
using Dolany.Ai.Util;

namespace Dolany.Test.Information
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void SendBtn_Click(object sender, EventArgs e)
        {
            var inforamtion = new MsgInformation
            {
                AiNum = long.Parse(UtTools.GetConfig("SelfQQNum")),
                FromGroup = long.Parse(GroupNumTxt.Text),
                FromQQ = long.Parse(QQNumTxt.Text),
                Information = AiInformation.Message,
                Time = DateTime.Now,
                Msg = MsgTxt.Text
            };

            RabbitMQService.Instance.Send(inforamtion);
        }
    }
}
