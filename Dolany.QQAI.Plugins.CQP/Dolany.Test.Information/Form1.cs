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
                FromGroup = long.Parse(GroupNumTxt.Text),
                FromQQ = long.Parse(QQNumTxt.Text),
                Information = InformationType.Message,
                Msg = MsgTxt.Text,
                BindAi = BindAiTxt.Text
            };

            RabbitMQService.Instance.Send(inforamtion);
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
