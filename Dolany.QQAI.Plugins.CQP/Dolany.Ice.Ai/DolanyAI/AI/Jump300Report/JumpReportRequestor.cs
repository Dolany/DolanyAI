using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class JumpReportRequestor
    {
        private GroupMsgDTO MsgDTO;
        private Action<GroupMsgDTO, string> ReportCallBack;

        public JumpReportRequestor(GroupMsgDTO MsgDTO, Action<GroupMsgDTO, string> ReportCallBack)
        {
            this.MsgDTO = MsgDTO;
            this.ReportCallBack = ReportCallBack;

            this.MsgDTO.Msg = Utility.UrlCharConvert(this.MsgDTO.Msg);
        }

        public void Work()
        {
            try
            {
                var allList = GetAllList(MsgDTO.Msg);
                var allDetails = GetAllDetails(allList);

                var analyzer = new JumpReportAnalyzer(allList, allDetails);
                var report = analyzer.GenReport();

                ReportCallBack(MsgDTO, report);
            }
            catch (Exception ex)
            {
                ReportCallBack(MsgDTO, ex.Message + ex.StackTrace);
            }
        }

        private List<JumpDetailHtmlParser> GetAllDetails(List<JumpListHtmlParser> allList)
        {
            var allDetails = new List<JumpDetailHtmlParser>();
            foreach (var list in allList)
            {
                foreach (var match in list.Matches)
                {
                    var aimStr = $"http://300report.jumpw.com/match.html?id={match.DetailAddr}";
                    var requester = new HttpRequester();
                    var HtmlStr = requester.Request(aimStr);

                    var dp = new JumpDetailHtmlParser();
                    dp.Load(HtmlStr);

                    allDetails.Add(dp);
                }
            }

            return allDetails;
        }

        private List<JumpListHtmlParser> GetAllList(string name)
        {
            var list = new List<JumpListHtmlParser>();
            var requester = new HttpRequester();
            int count = 0;
            int idx = 0;

            do
            {
                var HtmlStr = requester.Request($"http://300report.jumpw.com/list.html?name={MsgDTO.Msg}&index={idx}");

                var listParser = new JumpListHtmlParser();
                listParser.Load(HtmlStr);

                count = listParser.Matches.Count;
                if (count == 0)
                {
                    break;
                }

                idx += count;
                list.Add(listParser);
            } while (true);

            return list;
        }
    }
}