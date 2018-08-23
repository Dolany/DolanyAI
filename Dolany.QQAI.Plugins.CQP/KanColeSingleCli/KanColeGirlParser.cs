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

                var RecordedTag = "";
                for (int i = 0; i + 3 < gnodes.Count; i += 3)
                {
                    if (gnodes[i + 1].InnerText.Contains(".mp3"))
                    {
                        kanColeGirlVoices.Add(new KanColeGirlVoice
                        {
                            Tag = RecordedTag,
                            Content = ParseContent(gnodes[i]),
                            VoiceUrl = ParseVoiceUrl(gnodes[i + 1])
                        });
                        i--;
                    }
                    else
                    {
                        var voice = new KanColeGirlVoice
                        {
                            Tag = ParseTag(gnodes[i]),
                            Content = ParseContent(gnodes[i + 1]),
                            VoiceUrl = ParseVoiceUrl(gnodes[i + 2])
                        };
                        kanColeGirlVoices.Add(voice);
                        RecordedTag = voice.Tag;
                    }
                }
            }

            SimplifyList();
        }

        private void SimplifyList()
        {
            foreach (var voice in kanColeGirlVoices)
            {
                voice.Content = voice.Content.Replace("</p>", "");
                voice.Content = voice.Content.Replace("</div>", "");
                voice.Content = voice.Content.Replace("<br>", "");
                voice.Content = voice.Content.Replace(" ", "");
            }
        }

        private string ParseTag(HtmlNode node)
        {
            var text = node.InnerText;
            text = text.Replace("\n", "");
            text = text.Replace(" ", "");
            return text;
        }

        private string ParseContent(HtmlNode node)
        {
            if (node.InnerHtml.Contains("</span><br>"))
            {
                var strs = node.InnerHtml.Split(new string[] { "</span><br>" }, StringSplitOptions.RemoveEmptyEntries);
                return strs[1];
            }
            else
            {
                var strs = node.InnerHtml.Split(new string[] { "<p>" }, StringSplitOptions.RemoveEmptyEntries);
                return strs[2].Replace("<br>", "");
            }
        }

        private string ParseVoiceUrl(HtmlNode node)
        {
            // TODO
            return "";
        }
    }
}