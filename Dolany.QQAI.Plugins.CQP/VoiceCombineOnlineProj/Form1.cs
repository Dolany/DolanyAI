using System;
using System.Windows.Forms;
using Dolany.Ai.Reborn.DolanyAI.Model;
using static Dolany.Ai.Reborn.DolanyAI.Common.Utility;

namespace VoiceCombineOnlineProj
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void MakeBtn_Click(object sender, EventArgs e)
        {
            var songName = inputTxt.Text;
            var param = new PostReq_Param
            {
                InterfaceName = "http://api.xfyun.cn/v1/service/v1/tts",
                data = new XfyunRequest
                {
                    aue = "raw",
                    auf = "audio/L16;rate=16000",
                    voice_name = "xiaoyan"
                }
            };

            XfyunRequestHelper.PostData(param, new RequestBody { text = UrlCharConvert(songName) });
        }
    }
}