using System;
using System.Collections.Generic;
using Dolany.Ai.Reborn.DolanyAI.Ai.Game.Jump300Report.Parser;
using Dolany.Ai.Reborn.DolanyAI.DTO;
using Dolany.Ai.Reborn.DolanyAI.Net;
using static Dolany.Ai.Reborn.DolanyAI.Common.Utility;

namespace Dolany.Ai.Reborn.DolanyAI.Ai.Game.Jump300Report
{
    public class JumpReportRequestor
    {
        private readonly ReceivedMsgDTO MsgDTO;
        private Action<ReceivedMsgDTO, string> ReportCallBack { get; set; }

        public JumpReportRequestor(ReceivedMsgDTO MsgDTO, Action<ReceivedMsgDTO, string> ReportCallBack)
        {
            this.MsgDTO = MsgDTO;
            this.ReportCallBack = ReportCallBack;

            this.MsgDTO.Msg = UrlCharConvert(this.MsgDTO.Msg);
        }

        public void Work()
        {
            try
            {
                var allList = GetAllList();
                var allDetails = GetAllDetails(allList);

                var analyzer = new JumpReportAnalyzer(allList, allDetails);
                var report = analyzer.GenReport();

                ReportCallBack?.Invoke(MsgDTO, report);
            }
            catch (Exception ex)
            {
                ReportCallBack?.Invoke(MsgDTO, ex.Message + ex.StackTrace);
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
                    using (var requester = new HttpRequester())
                    {
                        var HtmlStr = requester.Request(aimStr);

                        var dp = new JumpDetailHtmlParser();
                        dp.Load(HtmlStr);

                        allDetails.Add(dp);
                    }
                }
            }

            return allDetails;
        }

        private List<JumpListHtmlParser> GetAllList()
        {
            var list = new List<JumpListHtmlParser>();
            using (var requester = new HttpRequester())
            {
                var idx = 0;

                do
                {
                    var HtmlStr = requester.Request($"http://300report.jumpw.com/list.html?name={MsgDTO.Msg}&index={idx}");

                    var listParser = new JumpListHtmlParser();
                    listParser.Load(HtmlStr);

                    var count = listParser.Matches.Count;
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
}
