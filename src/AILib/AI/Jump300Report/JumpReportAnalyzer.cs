using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            string report = string.Empty;
            report += SummaryReport();
            return report;
        }

        private string SummaryReport()
        {
            if (Lists == null || Lists.Count == 0)
            {
                return string.Empty;
            }

            string report = "基础信息：";
            foreach (var r in Lists.FirstOrDefault().BaseInfo)
            {
                report += '\r' + r.Name + ":" + r.Value;
            }

            report += '\r' + FavoriteHeroInfo();

            return report;
        }

        private string FavoriteHeroInfo()
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

            return $"最常用的英雄为：{FavoriteHero}   场次：{dic[FavoriteHero]}";
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