using System.Collections.Generic;
using System.Linq;
using Dolany.Ice.Ai.DolanyAI;
using HtmlAgilityPack;

namespace KanColeVoiceClimber
{
    public class ColeGirlListPageParse : HtmlParser
    {
        public List<string> GirlList = new List<string>();

        protected override void Parse()
        {
            base.Parse();

            var root = document.DocumentNode;
            var parentNodes = SearchNodes(
                root,
                p => p.Name == "table"
                && !p.ChildAttributes("class").IsNullOrEmpty()
                && p.ChildAttributes("class").First().Value.Contains("wikitable"));
            foreach (HtmlNode pNode in parentNodes)
            {
                var nodes = SearchNodes(
                pNode,
                p => p.Name == "a"
                && !p.ChildAttributes("href").IsNullOrEmpty()
                && !p.ChildAttributes("title").IsNullOrEmpty()
                && p.ChildAttributes("title").First().Value.Contains("舰队Collection:")
                );

                foreach (HtmlNode node in nodes)
                {
                    GirlList.Add(node.ChildAttributes("title").First().Value);
                }
            }

            GirlList = GirlList.Distinct().ToList();
        }
    }
}