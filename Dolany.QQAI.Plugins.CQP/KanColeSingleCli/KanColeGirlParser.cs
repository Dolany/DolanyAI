using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolany.Ice.Ai.DolanyAI;
using HtmlAgilityPack;
using Dolany.Ice.Ai.DolanyAI.Db;

namespace KanColeSingleCli
{
    public class KanColeGirlParser : HtmlParser
    {
        public List<KanColeGirlVoice> kanColeGirlVoices = new List<KanColeGirlVoice>();

        protected override void Parse()
        {
            base.Parse();

            var root = document.DocumentNode;

            var parentNode = SearchNodes(
                root,
                p => p.Name == "table"
                && !p.ChildAttributes("class").IsNullOrEmpty()
                && p.ChildAttributes("class").First().Value == "wikitable"
                );

            foreach (var node in parentNode)
            {
                var rowNodes = SearchNodes(
                node,
                p => p.Name == "a"
                && !p.ChildAttributes("data-filesrc").IsNullOrEmpty()
                && p.ChildAttributes("data-filesrc").First().Value.Contains(".mp3")
                );
                if (rowNodes.Count == 0)
                {
                    continue;
                }

                var gnodes = SearchNodes(node, p => p.Name == "td");

                for (int i = 0; i + 3 < gnodes.Count; i += 3)
                {
                    kanColeGirlVoices.Add(new KanColeGirlVoice
                    {
                        Tag = ParseTag(gnodes[i]),
                        Content = ParseContent(gnodes[i + 1]),
                        VoiceUrl = ParseVoiceUrl(gnodes[i + 2])
                    });
                }
            }
        }

        private string ParseTag(HtmlNode node)
        {
            // TODO
            return "";
        }

        private string ParseContent(HtmlNode node)
        {
            // TODO
            return "";
        }

        private string ParseVoiceUrl(HtmlNode node)
        {
            // TODO
            return "";
        }
    }
}