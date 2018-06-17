using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib.Html;

namespace AILib.AI.Jump300Report
{
    public class JumpListHtmlParser : HtmlParser
    {
        public JumpBaseInfo BaseInfo { get; private set; }
        public List<JumpServerRankInfo> RankInfos { get; private set; }
        public List<JumpMatchListInfo> Matches { get; private set; }

        protected override void Parse()
        {
        }
    }
}