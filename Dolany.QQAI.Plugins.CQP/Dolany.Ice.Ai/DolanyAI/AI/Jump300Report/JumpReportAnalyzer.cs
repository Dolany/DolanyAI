using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class JumpReportAnalyzer
    {
        public List<JumpListHtmlParser> Lists { get; set; }
        public List<JumpDetailHtmlParser> Details { get; set; }

        public string PlayerName
        {
            get
            {
                return Lists.FirstOrDefault().BaseInfo.Where(b => b.Name == "角色名").FirstOrDefault().Value;
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

            string report = string.Empty;

            var query = GetReportMethods();
            foreach (var m in query)
            {
                report += GenMethodReport(m);
            }

            return report;
        }

        private IOrderedEnumerable<MethodInfo> GetReportMethods()
        {
            Type t = this.GetType();
            var query = t.GetMethods()
                .Where(m => m.CustomAttributes.Any(a => a.AttributeType == typeof(JumpAnalyzeAttribute)))
                .OrderBy(m => (m.GetCustomAttributes(typeof(JumpAnalyzeAttribute), false).FirstOrDefault() as JumpAnalyzeAttribute).Order);

            return query;
        }

        private string GenMethodReport(MethodInfo m)
        {
            string Title = (m.GetCustomAttributes(typeof(JumpAnalyzeAttribute), false).FirstOrDefault() as JumpAnalyzeAttribute).Title;
            string content = m.DeclaringType.InvokeMember(m.Name,
                        BindingFlags.InvokeMethod,
                        null,
                        this,
                        null
                        ) as string;
            string report = $@"{Title} :
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

            string report = string.Empty;
            foreach (var r in Lists.FirstOrDefault().BaseInfo)
            {
                report += '\r' + r.Name + ":" + r.Value;
            }

            return report;
        }

        [JumpAnalyze(Order = 2, Title = "最常用的英雄")]
        public string FavoriteHeroInfo()
        {
            if (Lists.IsNullOrEmpty())
            {
                return string.Empty;
            }

            Dictionary<string, int> dic = DicHeros();

            string FavoriteHero = dic.Keys.FirstOrDefault();
            foreach (var k in dic.Keys)
            {
                if (dic[k] > dic[FavoriteHero])
                {
                    FavoriteHero = k;
                }
            }

            return $"{FavoriteHero}   场次：{dic[FavoriteHero]}";
        }

        private Dictionary<string, int> DicHeros()
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
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
            int gold = 0;
            int validMatch = 0;
            string playerName = PlayerName;
            foreach (var detail in Details)
            {
                var query = detail.PlayersInfo.Where(p => p.PlayerName == playerName);
                if (query.IsNullOrEmpty())
                {
                    continue;
                }

                int g = query.FirstOrDefault().MoneyGen;
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
            TimeSpan span = new TimeSpan(0);
            foreach (var d in Details)
            {
                span += d.MatchBaseInfo.DuringSpan;
            }

            long avgSpan = (long)((span.TotalMilliseconds / Details.Count()) * 10000);
            return (new TimeSpan(avgSpan)).ToString(@"hh\:mm\:ss");
        }

        [JumpAnalyze(Order = 5, Title = "场均评分")]
        public string AverageGrade()
        {
            int grade = 0;
            string playerName = PlayerName;
            foreach (var detail in Details)
            {
                if (detail.MatchBaseInfo.MatchKind == "战场")
                {
                    continue;
                }

                var query = detail.PlayersInfo.Where(p => p.PlayerName == PlayerName);
                if (query.IsNullOrEmpty())
                {
                    continue;
                }

                grade += query.FirstOrDefault().Grade;
            }

            return (grade / Details.Count()).ToString();
        }
    }
}