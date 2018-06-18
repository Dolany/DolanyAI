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
            // TODO
            return string.Empty;
        }
    }
}