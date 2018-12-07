using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace Dolany.Ai.Core.Ai.Game.Jump300Report.Parser
{
    using Dolany.Ai.Core.Ai.Game.Jump300Report.Model;
    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Net;

    public class JumpListHtmlParser : HtmlParser
    {
        public List<JumpBaseInfo> BaseInfo { get; }

        public List<JumpMatchBriefInfo> Matches { get; }

        public JumpListHtmlParser()
        {
            BaseInfo = new List<JumpBaseInfo>();
            Matches = new List<JumpMatchBriefInfo>();
        }

        protected override void Parse()
        {
            var root = document.DocumentNode;

            var nodes = SearchNodes(root, n => n.Name == "table");
            if (nodes.IsNullOrEmpty())
            {
                return;
            }
            ParseBaseInfo(nodes[0].InnerText);
            if (nodes.Count == 2)
            {
                ParseMatches(nodes[1].InnerText);
                AppendAddr(nodes[1]);
            }
            else
            {
                ParseRankInfos(nodes[1].InnerText);
                ParseMatches(nodes[2].InnerText);
                AppendAddr(nodes[2]);
            }
        }

        private void ParseBaseInfo(string text)
        {
            var strs = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in strs)
            {
                var kvs = s.Split(':');
                var key = kvs[0];
                var value = s.Substring(key.Length + 1, s.Length - key.Length - 1);

                BaseInfo.Add(new JumpBaseInfo
                {
                    Name = key,
                    Value = value
                });
            }
        }

        private void ParseRankInfos(string text)
        {
            var strs = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in strs)
            {
                var rankInfo = new JumpServerRankInfo();

                var ps1 = s.Split(new[] { '第', '名' }, StringSplitOptions.RemoveEmptyEntries);
                rankInfo.RankName = ps1[0];
                rankInfo.RankValue = int.Parse(ps1[1]);

                var ps2 = ps1[2].Split(new[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                var ps21 = ps2[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (ps21[0] == "↑")
                {
                    rankInfo.ChangeValue = int.Parse(ps21[1]);
                }
                else
                {
                    rankInfo.ChangeValue = int.Parse(ps21[1]) * -1;
                }

                var idx = NumStartIndex(ps2[2]);
                rankInfo.DataName = ps2[2].Substring(0, idx);
                rankInfo.Data = int.Parse(ps2[2].Substring(idx, ps2[2].Length - idx));
            }
        }

        private int NumStartIndex(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] >= '0' && str[i] <= '9')
                {
                    return i;
                }
            }

            return -1;
        }

        private void ParseMatches(string text)
        {
            var strs = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var idxes = FindEmptyIdxes(strs);
            foreach (var i in idxes)
            {
                var matchInfo = new JumpMatchBriefInfo
                {
                    MatchMode = strs[i + 1]
                };
                var sp = strs[i + 2].Split(new[] { "(", ")", "Lv." }, StringSplitOptions.RemoveEmptyEntries);
                matchInfo.HeroName = sp[0];
                matchInfo.Level = int.Parse(sp[1]);
                matchInfo.Result = strs[i + 3];
                matchInfo.Time = DateTime.Parse(strs[i + 4]);

                Matches.Add(matchInfo);
            }
        }

        private int[] FindEmptyIdxes(string[] strs)
        {
            var list = new List<int>();
            for (int i = 0; i < strs.Length; i++)
            {
                if (string.IsNullOrEmpty(strs[i].Trim()))
                {
                    list.Add(i);
                }
            }

            return list.ToArray();
        }

        private void AppendAddr(HtmlNode node)
        {
            var nodes = SearchNodes(node, n => n.ChildAttributes("onClick").Count() > 0);
            var addrList = new List<string>();
            foreach (var n in nodes)
            {
                var attr = n.ChildAttributes("onClick").FirstOrDefault();
                if (attr != null)
                {
                    var attrValue = attr.Value;
                    var addrName = attrValue.Replace("javascript:window.open('match.html?id=", "").Replace("');", "");
                    addrList.Add(addrName);
                }
            }

            for (var i = 0; i < Matches.Count(); i++)
            {
                Matches[i].DetailAddr = addrList[i];
            }
        }
    }
}
