using System;
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
                    var strs = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (strs.Length != 3)
                    {
                        throw new Exception(line);
                    }

                    using (AIDatabase db = new AIDatabase())
                    {
                        db.KanColeGirlVoice.Add(new KanColeGirlVoice
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "舰队Collection:长门",
                            VoiceUrl = strs[2],
                            Content = strs[1],
                            Tag = strs[0]
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
            Invoke(new Action(() =>
            {
                ShowTxt.AppendText(text + "\r\n");
            }));
        }

        // ReSharper disable once UnusedMember.Local
        private void ParseAGirl(string name)
        {
            AppendTxt($"正在解析{name}...");
            using (var requester = new HttpRequester())
            {
                AppendTxt("请求列表页面中...");
                var aimStr = $"https://zh.moegirl.org/{Utility.UrlCharConvert(name)}";
                var HtmlStr = requester.Request(aimStr);

                AppendTxt("解析列表页面中...");

                var parser = new KanColeGirlParser();
                parser.Load(HtmlStr);

                var list = parser.kanColeGirlVoices;
                AppendTxt($"共找到{list.Count}个语音信息，正在同步到数据库...");
                foreach (var _ in list)
                {
                    SynToDb();
                }
                AppendTxt("同步完成！");
            }
        }

        private static void SynToDb()
        {
        }
    }
}