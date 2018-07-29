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

namespace KanColeVoiceClimber
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => Work());
        }

        private void Work()
        {
            HttpRequester requester = new HttpRequester();

            AppendTxt("请求列表页面中...");
            string aimStr = $"https://zh.moegirl.org/zh-hans/%E8%88%B0%E9%98%9FCollection/%E5%9B%BE%E9%89%B4/%E8%88%B0%E5%A8%98";
            string HtmlStr = requester.Request(aimStr);

            AppendTxt("解析列表页面中...");
            ColeGirlListPageParse parser = new ColeGirlListPageParse();
            parser.Load(HtmlStr);

            var list = parser.GirlList;
            AppendTxt($"共找到{list.Count}个舰娘");
            foreach (var name in list)
            {
                ParseAGirl(name);
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
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.KanColeGirlVoice.Where(k => k.Tag == voice.Tag && k.Name == voice.Name);
                if (query.IsNullOrEmpty())
                {
                    db.KanColeGirlVoice.Add(voice);
                }
                else
                {
                    var kan = query.First();
                    kan.Content = voice.Content;
                    kan.VoiceUrl = voice.VoiceUrl;
                }
                db.SaveChanges();
            }
        }
    }
}