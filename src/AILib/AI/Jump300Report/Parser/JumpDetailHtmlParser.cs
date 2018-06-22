using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib.Html;
using HtmlAgilityPack;

namespace AILib.AI.Jump300Report
{
    public class JumpDetailHtmlParser : HtmlParser
    {
        public JumpMatchBaseInfo MatchBaseInfo { get; set; }

        public JumpDetailHtmlParser()
        {
            MatchBaseInfo = new JumpMatchBaseInfo();
        }

        protected override void Parse()
        {
            var root = document.DocumentNode;

            MatchBaseInfoAnalyze(root);
        }

        private void MatchBaseInfoAnalyze(HtmlNode root)
        {
            var query = SearchNodes(root,
                (n) => n.Name == "div"
                && n.ChildAttributes("class").Any((p) => p.Value == "datamsg"));
            if (query == null || query.Count() == 0)
            {
                return;
            }

            GenMatchBaseInfo(query.FirstOrDefault());
        }

        private void GenMatchBaseInfo(HtmlNode node)
        {
            string text = node.InnerText;
            string[] sp1 = text.Split(new string[] { ">>", ":", " ", "\r\n", "/", "-", "分", "秒" }, StringSplitOptions.RemoveEmptyEntries);

            MatchBaseInfo.MatchKind = sp1[1];
            MatchBaseInfo.TotalKill = int.Parse(sp1[3]);
            MatchBaseInfo.TotalDie = int.Parse(sp1[4]);
            MatchBaseInfo.EndTime = new DateTime(
                int.Parse(sp1[6]),
                int.Parse(sp1[7]),
                int.Parse(sp1[8]),
                int.Parse(sp1[9]),
                int.Parse(sp1[10]),
                int.Parse(sp1[11])
                );
            MatchBaseInfo.DuringSpan = new TimeSpan(0, int.Parse(sp1[13]), int.Parse(sp1[14]));
        }
    }
}