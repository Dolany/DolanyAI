using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace Dolany.Ai.Core.Ai.Game.Jump300Report.Parser
{
    using System.Linq;

    using Model;
    using Common;
    using Net;

    public class JumpDetailHtmlParser : HtmlParser
    {
        public JumpMatchBaseInfo MatchBaseInfo { get; }
        public List<PlayerInfoInMatch> PlayersInfo { get; }

        public JumpDetailHtmlParser()
        {
            MatchBaseInfo = new JumpMatchBaseInfo();
            PlayersInfo = new List<PlayerInfoInMatch>();
        }

        protected override void Parse()
        {
            var root = document.DocumentNode;

            MatchBaseInfoAnalyze(root);
            PlayersInfoAnalyze(root);
        }

        private void PlayersInfoAnalyze(HtmlNode root)
        {
            var query = SearchNodes(root, n => n.Name == "table");
            if (query.IsNullOrEmpty())
            {
                return;
            }

            foreach (var n in query)
            {
                GenPlayerInfo(n);
            }
        }

        private void GenPlayerInfo(HtmlNode tableNode)
        {
            var sp1 = tableNode.InnerText.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var cycleStep = MatchBaseInfo.MatchKind == "战场" ? 8 : 10;
            for (int i = 1; i + cycleStep <= sp1.Length; i += cycleStep)
            {
                PlayersInfo.Add(ParsePlayer(sp1, i));
            }
        }

        private PlayerInfoInMatch ParsePlayer(string[] sp, int startIdx)
        {
            var piim = new PlayerInfoInMatch();

            var sp1 = sp[startIdx].Split(new[] { "(", ")", "lv." }, StringSplitOptions.RemoveEmptyEntries);
            piim.PlayerName = sp1[0];
            piim.PlayerLevel = int.Parse(sp1[1]);
            piim.HeroName = sp1[2];
            piim.HeroLevel = int.Parse(sp1[3]);

            var sp2 = sp[startIdx + 1].Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            piim.Kill = int.Parse(sp2[0]);
            piim.Die = int.Parse(sp2[1]);
            piim.Assist = int.Parse(sp2[2]);

            piim.Result = sp[startIdx + 2];
            piim.BuildingDestory = int.Parse(sp[startIdx + 3]);
            piim.SoldierKill = int.Parse(sp[startIdx + 4]);
            if (MatchBaseInfo.MatchKind == "战场")
            {
                piim.MoneyGen = -1;
                piim.Grade = -1;
                startIdx -= 2;
            }
            else
            {
                piim.MoneyGen = int.Parse(sp[startIdx + 5]);
                piim.Grade = int.Parse(sp[startIdx + 6]);
            }

            var sp3 = sp[startIdx + 7].Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            piim.PlayerCoin = int.Parse(sp3[0]);
            piim.PlayerExp = int.Parse(sp3[1]);

            piim.MoralIntegrity = int.Parse(sp[startIdx + 8]);

            var sp4 = sp[startIdx + 9].Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            piim.TotalWin = int.Parse(sp4[0]);
            piim.TotalMatch = int.Parse(sp4[1]);

            return piim;
        }

        private void MatchBaseInfoAnalyze(HtmlNode root)
        {
            var query = SearchNodes(root,
                (n) => n.Name == "div"
                && n.ChildAttributes("class").Any((p) => p.Value == "datamsg"));
            if (query.IsNullOrEmpty())
            {
                return;
            }

            GenMatchBaseInfo(query.FirstOrDefault());
        }

        private void GenMatchBaseInfo(HtmlNode node)
        {
            var text = node.InnerText;
            var sp1 = text.Split(new[] { ">>", ":", " ", "\r\n", "/", "-" }, StringSplitOptions.RemoveEmptyEntries);

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
            MatchBaseInfo.DuringSpan = GenSpan(sp1[13]);
        }

        private TimeSpan GenSpan(string timeStr)
        {
            var hour = 0;
            var minute = 0;
            var second = 0;

            if (timeStr.Contains("小时"))
            {
                var sp1 = timeStr.Split(new[] { "小时" }, StringSplitOptions.RemoveEmptyEntries);
                hour = int.Parse(sp1[0]);
                timeStr = sp1[1];
            }

            if (timeStr.Contains("分"))
            {
                var sp2 = timeStr.Split(new[] { "分" }, StringSplitOptions.RemoveEmptyEntries);
                minute = int.Parse(sp2[0]);
                timeStr = sp2[1];
            }

            if (timeStr.Contains("秒"))
            {
                var sp3 = timeStr.Split(new[] { "秒" }, StringSplitOptions.RemoveEmptyEntries);
                second = int.Parse(sp3[0]);
            }

            return new TimeSpan(hour, minute, second);
        }
    }
}
