using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Dolany.Ice.Ai.DolanyAI;
using Dolany.Ice.Ai.DolanyAI.Db;
using System.IO;

namespace KanColeSingleCli
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            ParseAGirl(NameTxt.Text);
        }

        private void ParseAGirl(string name)
        {
            AppendTxt($"正在解析{name}...");
            using (var requester = new HttpRequester())
            {
                AppendTxt("请求列表页面中...");
                var aimStr = UrlTxt.Text;
                var HtmlStr = requester.Request(aimStr);

                AppendTxt("解析列表页面中...");

                var parser = new KanColeGirlParser();
                parser.Load(HtmlStr);

                var list = parser.kanColeGirlVoices;
                AppendTxt($"共找到{list.Count}个语音信息，正在写入文件...");

                WriteToFile(list);
                AppendTxt("同步完成！");
            }
        }

        private void AppendTxt(string text)
        {
            Invoke(new Action(() =>
            {
                LogTxt.AppendText(text + "\r\n");
            }));
        }

        private void WriteToFile(List<KanColeGirlVoice> voices)
        {
            using (var fs = new FileStream(AimTxt.Text + NameTxt.Text + ".txt", FileMode.Create))
            {
                using (var sw = new StreamWriter(fs))
                {
                    foreach (var voice in voices)
                    {
                        sw.Write($"{voice.Tag} {voice.Content} {voice.VoiceUrl} \r\n");
                    }

                    sw.Flush();
                }
            }
        }
    }
}