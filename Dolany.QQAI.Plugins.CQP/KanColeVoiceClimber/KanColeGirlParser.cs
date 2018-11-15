using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Reborn.DolanyAI.Common;
using Dolany.Ai.Reborn.DolanyAI.Db;
using Dolany.Ai.Reborn.DolanyAI.Net;
using HtmlAgilityPack;

namespace KanColeVoiceClimber
{
    public class KanColeGirlParser : HtmlParser
    {
        public List<KanColeGirlVoice> kanColeGirlVoices = new List<KanColeGirlVoice>();
        private string GirlName;

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
        }

        private static List<string> GetDescList(IEnumerable<HtmlNode> outerNodes)
        {
            var list = new List<string>();
            foreach (var node in outerNodes)
            {
                var tdNode = node.ChildNodes.First(p => p.InnerText.Contains("00"));
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
            var strs = descStr.Split(new[] { '：' }, StringSplitOptions.RemoveEmptyEntries);
            if (strs.IsNullOrEmpty()
                || strs.Length != 3
                )
            {
                return;
            }

            var voice = new KanColeGirlVoice
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