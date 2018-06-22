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
        public List<PlayerInfoInMatch> PlayersInfo { get; set; }

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
            if (query == null || query.Count() == 0)
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
            string[] sp1 = tableNode.InnerText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i + 10 <= sp1.Length; i += 10)
            {
                PlayersInfo.Add(ParsePlayer(sp1, i));
            }
        }

        private PlayerInfoInMatch ParsePlayer(string[] sp, int startIdx)
        {
            PlayerInfoInMatch piim = new PlayerInfoInMatch();

            string[] sp1 = sp[startIdx].Split(new string[] { "(", ")", "lv." }, StringSplitOptions.RemoveEmptyEntries);
            piim.PlayerName = sp1[0];
            piim.PlayerLevel = int.Parse(sp1[1]);
            piim.HeroName = sp1[2];
            piim.HeroLevel = int.Parse(sp1[3]);

            string[] sp2 = sp[startIdx + 1].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            piim.Kill = int.Parse(sp2[0]);
            piim.Die = int.Parse(sp2[1]);
            piim.Assist = int.Parse(sp2[2]);

            piim.Result = sp[startIdx + 2];
            piim.BuildingDestory = int.Parse(sp[startIdx + 3]);
            piim.SoldierKill = int.Parse(sp[startIdx + 4]);
            piim.MoneyGen = int.Parse(sp[startIdx + 5]);
            piim.Grade = int.Parse(sp[startIdx + 6]);

            string[] sp3 = sp[startIdx + 7].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            piim.PlayerCoin = int.Parse(sp3[0]);
            piim.PlayerExp = int.Parse(sp3[1]);

            piim.MoralIntegrity = int.Parse(sp[startIdx + 8]);

            string[] sp4 = sp[startIdx + 9].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            piim.TotalWin = int.Parse(sp4[0]);
            piim.TotalMatch = int.Parse(sp4[1]);

            return piim;
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