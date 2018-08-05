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
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            fileName = dialog.FileName;

            Task.Factory.StartNew(() => Work());
        }

        private void Work()
        {
            //HttpRequester requester = new HttpRequester();

            //AppendTxt("请求列表页面中...");
            //string aimStr = $"https://zh.moegirl.org/zh-hans/%E8%88%B0%E9%98%9FCollection/%E5%9B%BE%E9%89%B4/%E8%88%B0%E5%A8%98";
            //string HtmlStr = requester.Request(aimStr);

            //AppendTxt("解析列表页面中...");
            //ColeGirlListPageParse parser = new ColeGirlListPageParse();
            //parser.Load(HtmlStr);

            //var list = parser.GirlList;
            //AppendTxt($"共找到{list.Count}个舰娘");
            //foreach (var name in list)
            //{
            //    ParseAGirl(name);
            //}
            //ParseAGirl("舰队Collection:Верный");


            using (StreamReader reader = new StreamReader(fileName))
            {
                string line = reader.ReadLine();
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
            //using (AIDatabase db = new AIDatabase())
            //{
            //    var query = db.KanColeGirlVoice.Where(p => p.Name == "舰队Collection:响改二" && p.Tag == "母港/详细阅览（2016梅雨限定）");
            //    int count = query.Count();
            //    db.KanColeGirlVoice.RemoveRange(query);
            //    db.SaveChanges();
            //}

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
            HttpRequester requester = new HttpRequester();

            AppendTxt("请求列表页面中...");
            string aimStr = $"https://zh.moegirl.org/{Utility.UrlCharConvert(name)}";
            string HtmlStr = requester.Request(aimStr);

            AppendTxt("解析列表页面中...");

            KanColeGirlParser parser = new KanColeGirlParser();
            parser.Load(HtmlStr);

            var list = parser.kanColeGirlVoices;
            AppendTxt($"共找到{list.Count}个语音信息，正在同步到数据库...");
            foreach (var voice in list)
            {
                SynToDb(voice);
            }
            AppendTxt("同步完成！");
        }

        private void SynToDb(KanColeGirlVoice voice)
        {
            //using (AIDatabase db = new AIDatabase())
            //{
            //    var query = db.KanColeGirlVoice.Where(k => k.Tag == voice.Tag && k.Name == voice.Name);
            //    if (query.IsNullOrEmpty())
            //    {
            //        db.KanColeGirlVoice.Add(voice);
            //    }
            //    else
            //    {
            //        var kan = query.First();
            //        kan.Content = voice.Content;
            //        kan.VoiceUrl = voice.VoiceUrl;
            //    }
            //    db.SaveChanges();
            //}
        }
    }
}