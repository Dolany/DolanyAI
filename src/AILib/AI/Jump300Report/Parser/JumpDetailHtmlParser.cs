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
        }
    }
}