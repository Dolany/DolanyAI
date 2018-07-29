using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolany.Ice.Ai.DolanyAI;
using HtmlAgilityPack;
using Dolany.Ice.Ai.DolanyAI.Db;

namespace KanColeVoiceClimber
{
    public class KanColeGirlParser : HtmlParser
    {
        public List<KanColeGirlVoice> kanColeGirlVoices = new List<KanColeGirlVoice>();
        private string GirlName;

        private string[] timeStrs = {
            "0000", "0100", "0200", "0300", "0400", "0500",
            "0600", "0700", "0800", "0900", "1000", "1100",
            "1200", "1300", "1400", "1500", "1600", "1700",
            "1800", "1900", "2000", "2100", "2200", "2300"
        };

        protected override void Parse()
        {
            base.Parse();

            var root = document.DocumentNode;
            ParseTitle(root);
            var parentNode = SearchNodes(
                root,
                p => p.Name == "table"
                && !p.ChildAttributes("class").IsNullOrEmpty()
                && p.ChildAttributes("class").First().Value == "wikitable"
                );

            var rowNodes = SearchNodes(
                parentNode[1],
                p => p.Name == "a"
                && !p.ChildAttributes("data-filesrc").IsNullOrEmpty()
                && p.ChildAttributes("data-filesrc").First().Value.Contains(".mp3")
                );
            var outerNodes = rowNodes.Select(p => p.ParentNode
                                                   .ParentNode
                                                   .ParentNode
                                                   .ParentNode
                                                   .ParentNode
                                                   .ParentNode
                                                   .ParentNode
                                                   );
            var descList = GetDescList(outerNodes);
            for (int i = 0; i < rowNodes.Count; i++)
            {
                ParseInfo(rowNodes[i], descList[i]);
            }
        }

        private List<string> GetDescList(IEnumerable<HtmlNode> outerNodes)
        {
            List<string> list = new List<string>();
            foreach (var node in outerNodes)
            {
                var tdNode = node.ChildNodes[1];
                list.Add(tdNode.InnerText.Replace("\n", ""));
            }

            return list;
        }

        private void ParseTitle(HtmlNode root)
        {
            var title = SearchNodes(root, p => p.Name == "title").First();
            var strs = title.InnerText.Split(new char[] { ' ' });
            GirlName = strs[0];
        }

        private void ParseInfo(HtmlNode voiceNode, string descStr)
        {
            string[] strs = descStr.Split(new char[] { '：' }, StringSplitOptions.RemoveEmptyEntries);
            if (strs.IsNullOrEmpty()
                || strs.Length != 3
                )
            {
                return;
            }

            if (!timeStrs.Contains(strs[0].Trim()))
            {
                return;
            }

            KanColeGirlVoice voice = new KanColeGirlVoice
            {
                Id = Guid.NewGuid().ToString(),
                Content = strs[2].Trim(),
                VoiceUrl = voiceNode.ChildAttributes("data-filesrc").First().Value,
                Name = GirlName,
                Tag = strs[0].Trim()
            };

            kanColeGirlVoices.Add(voice);
        }
    }
}