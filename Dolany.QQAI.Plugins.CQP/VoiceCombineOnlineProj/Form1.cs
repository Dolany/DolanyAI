using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dolany.Ice.Ai.MahuaApis;
using Dolany.Ice.Ai.DolanyAI;

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
            string songName = inputTxt.Text;
            PostReq_Param param = new PostReq_Param
            {
                InterfaceName = "http://api.xfyun.cn/v1/service/v1/tts",
                data = new XfyunRequest
                {
                    aue = "raw",
                    auf = "audio/L16;rate=16000",
                    voice_name = "xiaoyan"
                }
            };

            var reponse = XfyunRequestHelper.PostData(param, new RequestBody { text = Utility.UrlCharConvert(songName) });
        }
    }
}