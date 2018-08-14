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
using Dolany.Ice.Ai.DolanyAI.Db;
using System.IO;

namespace KanColeVoiceClimber
{
    public partial class Form1 : Form
    {
        private string fileName;

        public Form1()
        {
            InitializeComponent();
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                fileName = dialog.FileName;

                Task.Factory.StartNew(() => Work());
            }
        }

        private void Work()
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                var line = reader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    var strs = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (strs.Length != 3)
                    {
                        throw new Exception(line);
                    }

                    using (AIDatabase db = new AIDatabase())
                    {
                        db.KanColeGirlVoice.Add(new KanColeGirlVoice
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "舰队Collection:响改二",
                            VoiceUrl = strs[0],
                            Content = strs[1],
                            Tag = strs[2]
                        });

                        db.SaveChanges();
                    }
                    AppendTxt(strs[1]);
                    line = reader.ReadLine();
                }
            }

            AppendTxt("任务结束！");
        }

        private void AppendTxt(string text)
        {
            this.Invoke(new Action(() =>
            {
                ShowTxt.AppendText(text + "\r\n");
            }));
        }

        private void ParseAGirl(string name)
        {
            AppendTxt($"正在解析{name}...");
            var requester = new HttpRequester();

            AppendTxt("请求列表页面中...");
            var aimStr = $"https://zh.moegirl.org/{Utility.UrlCharConvert(name)}";
            var HtmlStr = requester.Request(aimStr);

            AppendTxt("解析列表页面中...");

            var parser = new KanColeGirlParser();
            parser.Load(HtmlStr);

            var list = parser.kanColeGirlVoices;
            AppendTxt($"共找到{list.Count}个语音信息，正在同步到数据库...");
            foreach (var voice in list)
            {
                SynToDb(voice);
            }
            AppendTxt("同步完成！");
        }

        private static void SynToDb(KanColeGirlVoice voice)
        {
        }
    }
}