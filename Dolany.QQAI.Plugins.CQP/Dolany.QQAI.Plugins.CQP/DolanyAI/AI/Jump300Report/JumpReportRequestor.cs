using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.QQAI.Plugins.CQP.DolanyAI
{
    public class JumpReportRequestor
    {
        private GroupMsgDTO MsgDTO;
        private Action<GroupMsgDTO, string> ReportCallBack;

        public JumpReportRequestor(GroupMsgDTO MsgDTO, Action<GroupMsgDTO, string> ReportCallBack)
        {
            this.MsgDTO = MsgDTO;
            this.ReportCallBack = ReportCallBack;

            this.MsgDTO.Msg = NameConvert(this.MsgDTO.Msg);
        }

        public void Work()
        {
            try
            {
                var allList = GetAllList(MsgDTO.Msg);
                var allDetails = GetAllDetails(allList);

                JumpReportAnalyzer analyzer = new JumpReportAnalyzer(allList, allDetails);
                string report = analyzer.GenReport();

                ReportCallBack(MsgDTO, report);
            }
            catch (Exception ex)
            {
                ReportCallBack(MsgDTO, ex.Message + ex.StackTrace);
            }
        }

        private List<JumpDetailHtmlParser> GetAllDetails(List<JumpListHtmlParser> allList)
        {
            List<JumpDetailHtmlParser> allDetails = new List<JumpDetailHtmlParser>();
            foreach (var list in allList)
            {
                foreach (var match in list.Matches)
                {
                    string aimStr = $"http://300report.jumpw.com/match.html?id={match.DetailAddr}";
                    HttpRequester requester = new HttpRequester();
                    string HtmlStr = requester.Request(aimStr);

                    JumpDetailHtmlParser dp = new JumpDetailHtmlParser();
                    dp.Load(HtmlStr);

                    allDetails.Add(dp);
                }
            }

            return allDetails;
        }

        private List<JumpListHtmlParser> GetAllList(string name)
        {
            List<JumpListHtmlParser> list = new List<JumpListHtmlParser>();
            HttpRequester requester = new HttpRequester();
            int count = 0;
            int idx = 0;

            do
            {
                string HtmlStr = requester.Request($"http://300report.jumpw.com/list.html?name={MsgDTO.Msg}&index={idx}");

                JumpListHtmlParser listParser = new JumpListHtmlParser();
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

        private string NameConvert(string name)
        {
            string result = string.Empty;
            foreach (var c in name)
            {
                if (IsAsciiChar(c))
                {
                    result += c;
                    continue;
                }

                result += "%" + BitConverter.ToString(Encoding.UTF8.GetBytes(new char[] { c })).Replace("-", "%");
            }

            return result;
        }

        private bool IsAsciiChar(char c)
        {
            return c >= 0x20 && c <= 0x7e;
        }
    }
}