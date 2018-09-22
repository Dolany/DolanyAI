using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ice.Ai.DolanyAI;
using HtmlAgilityPack;
using Dolany.Ice.Ai.DolanyAI.Db;
using Dolany.Ice.Ai.DolanyAI.Utils;

namespace KanColeSingleCli
{
    public class KanColeGirlParser : HtmlParser
    {
        public readonly List<KanColeGirlVoice> kanColeGirlVoices = new List<KanColeGirlVoice>();

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
                p => p.Name == "a" &&
                     !p.ChildAttributes("data-filesrc").IsNullOrEmpty() &&
                     p.ChildAttributes("data-filesrc").First().Value.Contains(".mp3")
                );
                if (rowNodes.Count == 0)
                {
                    continue;
                }

                var gnodes = SearchNodes(node, p => p.Name == "td");

                var recordedTag = "";
                for (var i = 0; i + 3 < gnodes.Count; i += 3)
                {
                    if (gnodes[i + 1].InnerText.Contains("mp3")
                        || gnodes[i + 1].InnerText.Contains(".oga")
                        || gnodes[i + 1].InnerText.Contains(".ogg"))
                    {
                        kanColeGirlVoices.Add(new KanColeGirlVoice
                        {
                            Tag = recordedTag,
                            Content = ParseContent(gnodes[i]),
                            VoiceUrl = ParseVoiceUrl(gnodes[i + 1])
                        });
                        i--;
                    }
                    else if (gnodes[i + 1].InnerText.Contains("暂缺"))
                    {
                        i--;
                    }
                    else if (gnodes[i + 2].InnerText.Contains("无") ||
                             gnodes[i + 2].InnerText == "\n")
                    {
                    }
                    else
                    {
                        try
                        {
                            var voice = new KanColeGirlVoice
                            {
                                Tag = ParseTag(gnodes[i]),
                                Content = ParseContent(gnodes[i + 1]),
                                VoiceUrl = ParseVoiceUrl(gnodes[i + 2])
                            };
                            kanColeGirlVoices.Add(voice);
                            recordedTag = voice.Tag;
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                }
            }

            SimplifyList();
        }

        private void SimplifyList()
        {
            foreach (var voice in kanColeGirlVoices)
            {
                voice.Content = voice.Content.Replace("<p>", "");
                voice.Content = voice.Content.Replace("</p>", "");
                voice.Content = voice.Content.Replace("</div>", "");
                voice.Content = voice.Content.Replace("<br>", "");
                voice.Content = voice.Content.Replace(" ", "");
                voice.Content = voice.Content.Replace("\n", "");

                voice.Content = RemoveBetween(voice.Content, "<spanclass", "</span>");
                voice.Content = RemoveBetween(voice.Content, "<spanlang=\"ja\"", "</span>");
                voice.Content = RemoveBetween(voice.Content, "<del>", "</del>");
                voice.Content = RemoveBetween(voice.Content, "<s>", "</s>");
                voice.Content = RemoveBetween(voice.Content, "<ul>", "</ul>");
                voice.Content = RemoveBetween(voice.Content, "<sup", "</sup>");
                voice.Content = RemoveBetween(voice.Content, "<spanclass=\"heimu\"", "</span>");
            }
        }

        private string RemoveBetween(string content, string start, string end)
        {
            var sidx = content.IndexOf(start, StringComparison.Ordinal);
            if (sidx < 0)
            {
                return content;
            }
            var eidx = content.IndexOf(end, sidx, StringComparison.Ordinal);
            if (eidx < 0)
            {
                return content;
            }
            return content.Remove(sidx, eidx - sidx + end.Length);
        }

        private static string ParseTag(HtmlNode node)
        {
            var text = node.InnerText;
            text = text.Replace("\n", "");
            text = text.Replace(" ", "");
            return text;
        }

        private static string ParseContent(HtmlNode node)
        {
            if (node.InnerHtml.Contains("</span><br>"))
            {
                var strs = node.InnerHtml.Split(new[] { "</span><br>" }, StringSplitOptions.RemoveEmptyEntries);
                return strs[1];
            }
            else
            {
                var strs = node.InnerHtml.Split(new[] { "<p>" }, StringSplitOptions.RemoveEmptyEntries);
                return strs.Last();
            }
        }

        private string ParseVoiceUrl(HtmlNode node)
        {
            var rowNodes = SearchNodes(
                node,
                p => p.Name == "a"
                && !p.ChildAttributes("data-filesrc").IsNullOrEmpty()
                && (p.ChildAttributes("data-filesrc").First().Value.Contains("mp3")
                     || p.ChildAttributes("data-filesrc").First().Value.Contains(".oga")
                     || p.ChildAttributes("data-filesrc").First().Value.Contains(".ogg"))
                );
            return rowNodes.First().ChildAttributes("data-filesrc").First().Value;
        }
    }
}