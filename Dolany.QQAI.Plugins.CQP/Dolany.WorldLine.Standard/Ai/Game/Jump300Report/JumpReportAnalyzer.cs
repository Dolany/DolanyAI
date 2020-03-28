using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Dolany.Ai.Common;
using Dolany.WorldLine.Standard.Ai.Game.Jump300Report.Parser;

namespace Dolany.WorldLine.Standard.Ai.Game.Jump300Report
{
    public class JumpReportAnalyzer
    {
        private List<JumpListHtmlParser> Lists { get; }
        private List<JumpDetailHtmlParser> Details { get; }

        private string PlayerName
        {
            get
            {
                return Lists.FirstOrDefault()?.BaseInfo.FirstOrDefault(b => b.Name == "角色名")?.Value;
            }
        }

        public JumpReportAnalyzer(List<JumpListHtmlParser> Lists, List<JumpDetailHtmlParser> Details)
        {
            this.Lists = Lists;
            this.Details = Details;
        }

        public string GenReport()
        {
            if (Lists.IsNullOrEmpty())
            {
                return "欸呀呀，战绩查询不到呀！";
            }

            var report = string.Empty;

            var query = GetReportMethods();
            var builder = new StringBuilder();
            builder.Append(report);
            foreach (var m in query)
            {
                builder.Append(GenMethodReport(m));
            }
            report = builder.ToString();

            return report;
        }

        private IOrderedEnumerable<MethodInfo> GetReportMethods()
        {
            var t = GetType();
            var query = t.GetMethods()
                .Where(m => m.CustomAttributes.Any(a => a.AttributeType == typeof(JumpAnalyzeAttribute)))
                .OrderBy(m =>
                {
                    JumpAnalyzeAttribute jumpAnalyzeAttribute = m.GetCustomAttributes(typeof(JumpAnalyzeAttribute), false).FirstOrDefault() as JumpAnalyzeAttribute;
                    Debug.Assert(jumpAnalyzeAttribute != null, nameof(jumpAnalyzeAttribute) + " != null");
                    return jumpAnalyzeAttribute.Order;
                });

            return query;
        }

        private string GenMethodReport(MethodInfo m)
        {
            var Title = (m.GetCustomAttributes(typeof(JumpAnalyzeAttribute), false).FirstOrDefault() as JumpAnalyzeAttribute)?.Title;
            Debug.Assert(m.DeclaringType != null, "m.DeclaringType != null");
            var content = m.DeclaringType.InvokeMember(m.Name, BindingFlags.InvokeMethod, null, this, null) as string;
            var report = $@"{Title} :
{content}
";

            return report;
        }

        [JumpAnalyze(Order = 1, Title = "基础信息")]
        public string SummaryReport()
        {
            if (Lists.IsNullOrEmpty())
            {
                return string.Empty;
            }

            var report = string.Empty;
            var builder = new StringBuilder();
            builder.Append(report);
            Debug.Assert(Lists != null, nameof(Lists) + " != null");
            var jumpBaseInfos = Lists.FirstOrDefault()?.BaseInfo;
            if (jumpBaseInfos != null)
            {
                foreach (var r in jumpBaseInfos)
                {
                    builder.Append($"\r\n{r.Name}:{r.Value}");
                }
            }

            report = builder.ToString();

            return report;
        }

        [JumpAnalyze(Order = 2, Title = "最常用的英雄")]
        public string FavoriteHeroInfo()
        {
            if (Lists.IsNullOrEmpty())
            {
                return string.Empty;
            }

            var dic = DicHeros();

            var FavoriteHero = dic.Keys.FirstOrDefault();
            foreach (var k in dic.Keys)
            {
                if (dic[k] > dic[FavoriteHero ?? throw new InvalidOperationException()])
                {
                    FavoriteHero = k;
                }
            }

            return $"{FavoriteHero}   场次：{dic[FavoriteHero ?? throw new InvalidOperationException()]}";
        }

        private Dictionary<string, int> DicHeros()
        {
            var dic = new Dictionary<string, int>();
            foreach (var l in Lists)
            {
                foreach (var i in l.Matches)
                {
                    if (dic.Keys.Contains(i.HeroName))
                    {
                        dic[i.HeroName]++;
                    }
                    else
                    {
                        dic.Add(i.HeroName, 1);
                    }
                }
            }

            return dic;
        }

        [JumpAnalyze(Order = 3, Title = "平均打钱数")]
        public string AverageGoldGen()
        {
            var gold = 0;
            var validMatch = 0;
            var playerName = PlayerName;
            foreach (var detail in Details)
            {
                var query = detail.PlayersInfo.FirstOrDefault(p => p.PlayerName == playerName);
                if (query == null)
                {
                    continue;
                }

                var g = query.MoneyGen;
                if (g < 0)
                {
                    continue;
                }
                gold += g;
                validMatch++;
            }

            return (gold / validMatch).ToString();
        }

        [JumpAnalyze(Order = 4, Title = "平均比赛时长")]
        public string AverageMatchSpan()
        {
            var span = new TimeSpan(0);
            foreach (var d in Details)
            {
                span += d.MatchBaseInfo.DuringSpan;
            }

            var avgSpan = (long)(span.TotalMilliseconds / Details.Count * 10000);
            return (new TimeSpan(avgSpan)).ToString(@"hh\:mm\:ss");
        }

        [JumpAnalyze(Order = 5, Title = "场均评分")]
        public string AverageGrade()
        {
            var grade = 0;
            foreach (var detail in Details)
            {
                if (detail.MatchBaseInfo.MatchKind == "战场")
                {
                    continue;
                }

                var query = detail.PlayersInfo.FirstOrDefault(p => p.PlayerName == PlayerName);
                if (query == null)
                {
                    continue;
                }

                grade += query.Grade;
            }

            return (grade / Details.Count).ToString();
        }
    }
}
