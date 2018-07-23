using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.QQAI.Plugins.DolanyAI
{
    public class FortuneRequestor
    {
        private GroupMsgDTO MsgDTO;
        private Action<GroupMsgDTO, string> ReportCallBack;

        private Dictionary<string, int> StarMap = new Dictionary<string, int>();

        public FortuneRequestor(GroupMsgDTO MsgDTO, Action<GroupMsgDTO, string> ReportCallBack)
        {
            this.MsgDTO = MsgDTO;
            this.ReportCallBack = ReportCallBack;

            InitStarMap();
        }

        public void Work()
        {
            StarFortuneParser parser = new StarFortuneParser();

            HttpRequester requester = new HttpRequester();
            int code = GetStarCode(MsgDTO.Msg);
            if (code < 0)
            {
                ReportCallBack(MsgDTO, "未查找到该星座！");
                return;
            }

            string aimStr = $"http://astro.sina.cn/fortune/starent?type=day&ast={code}&vt=4";
            string HtmlStr = requester.Request(aimStr);

            parser.Load(HtmlStr);

            ReportCallBack(MsgDTO, parser.Content);
        }

        private int GetStarCode(string starName)
        {
            starName = starName.Replace("座", "");
            if (!StarMap.Keys.Contains(starName))
            {
                return -1;
            }

            return StarMap[starName];
        }

        private void InitStarMap()
        {
            StarMap.Clear();

            StarMap.Add("白羊", 1);
            StarMap.Add("金牛", 2);
            StarMap.Add("双子", 3);
            StarMap.Add("巨蟹", 4);
            StarMap.Add("狮子", 5);
            StarMap.Add("处女", 6);
            StarMap.Add("天秤", 7);
            StarMap.Add("天蝎", 8);
            StarMap.Add("射手", 9);
            StarMap.Add("摩羯", 10);
            StarMap.Add("水瓶", 11);
            StarMap.Add("双鱼", 12);
        }
    }
}