using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AILib.AI.Jump300Report
{
    public class JumpReportAnalyzer
    {
        public List<JumpListHtmlParser> Lists { get; set; }
        public List<JumpDetailHtmlParser> Details { get; set; }

        public JumpReportAnalyzer(List<JumpListHtmlParser> Lists, List<JumpDetailHtmlParser> Details)
        {
            this.Lists = Lists;
            this.Details = Details;
        }

        public string GenReport()
        {
            if (Lists == null || Lists.Count == 0)
            {
                return "欸呀呀，战绩查询不到呀！";
            }

            string report = string.Empty;

            Type t = this.GetType();
            var query = t.GetMethods()
                .Where(m => m.CustomAttributes.Any(a => a.AttributeType == typeof(JumpAnalyzeAttribute)))
                .OrderBy(m => (m.GetCustomAttributes(typeof(JumpAnalyzeAttribute), false).FirstOrDefault() as JumpAnalyzeAttribute).Order);
            foreach (var m in query)
            {
                string Title = (m.GetCustomAttributes(typeof(JumpAnalyzeAttribute), false).FirstOrDefault() as JumpAnalyzeAttribute).Title;
                string content = t.InvokeMember(m.Name,
                            BindingFlags.InvokeMethod,
                            null,
                            this,
                            null
                            ) as string;
                report += $@"{Title} :
{content}
";
            }

            return report;
        }

        [JumpAnalyze(Order = 1, Title = "基础信息")]
        public string SummaryReport()
        {
            if (Lists == null || Lists.Count == 0)
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
            if (Lists == null || Lists.Count == 0)
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
    }
}