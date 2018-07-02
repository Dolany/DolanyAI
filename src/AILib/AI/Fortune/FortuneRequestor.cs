using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib
{
    public class FortuneRequestor
    {
        private GroupMsgDTO MsgDTO;
        private Action<GroupMsgDTO, string> ReportCallBack;

        public FortuneRequestor(GroupMsgDTO MsgDTO, Action<GroupMsgDTO, string> ReportCallBack)
        {
            this.MsgDTO = MsgDTO;
            this.ReportCallBack = ReportCallBack;
        }

        public void Work()
        {
            StarFortuneParser parser = new StarFortuneParser();

            HttpRequester requester = new HttpRequester();
            string aimStr = $"http://astro.sina.cn/fortune/starent";
            string HtmlStr = requester.Request(aimStr);

            parser.Load(HtmlStr);
        }
    }
}