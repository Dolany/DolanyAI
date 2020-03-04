using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;
using Newtonsoft.Json;

namespace Dolany.WorldLine.Standard.Ai.SingleCommand.Fortune
{
    public class FortuneRequestor
    {
        private readonly MsgInformationEx MsgDTO;

        private readonly Dictionary<string, string> StarMap = new Dictionary<string, string>();

        private const string GetUrl = "https://interface.sina.cn/ast/get_app_fate.d.json";

        public FortuneRequestor(MsgInformationEx MsgDTO)
        {
            this.MsgDTO = MsgDTO;

            InitStarMap();
        }

        public void Work()
        {
            var star = MsgDTO.Msg.Replace("座", "");
            if (!StarMap.ContainsKey(star))
            {
                MsgSender.PushMsg(MsgDTO, "未找到该星座！");
                return;
            }

            var responseStr = CommonUtil.GetData(GetUrl, new Dictionary<string, string>() {{"type", "astro"}, {"class", StarMap[star]}});
            var data = JsonConvert.DeserializeObject<StarFortuneResponseModel>(responseStr).result.data["今日"];


            var msg = $"【{star}】\r";
            msg += $"{data.comment.name}:{data.comment.value}\r";
            msg += $"{string.Join("，", data.new_list.Select(p => $"{p.name}:{p.value}"))}\r";
            msg += $"{data.transit.name}:{data.transit.value}\r";
            msg += $"{data.new_content}";

            MsgSender.PushMsg(MsgDTO, msg, true);
        }

        private void InitStarMap()
        {
            StarMap.Clear();

            StarMap.Add("白羊", "Aries");
            StarMap.Add("金牛", "Taurus");
            StarMap.Add("双子", "Gemini");
            StarMap.Add("巨蟹", "Cancer");
            StarMap.Add("狮子", "Leo");
            StarMap.Add("处女", "Virgo");
            StarMap.Add("天秤", "Libra");
            StarMap.Add("天蝎", "Scorpio");
            StarMap.Add("射手", "Sagittarius");
            StarMap.Add("摩羯", "Capricorn");
            StarMap.Add("水瓶", "Aquarius");
            StarMap.Add("双鱼", "Pisces");
        }
    }

    public class StarFortuneResponseModel
    {
        public StarFortuneResponseResult result { get; set; }
    }

    public class StarFortuneResponseResult
    {
        public Dictionary<string, StarFortuneResponseData> data { get; set; }

        public object status { get; set; }
    }

    public class StarFortuneResponseData
    {
        public StarFortuneResponseDataUnit comment { get; set; }

        public StarFortuneResponseDataUnit[] new_list { get; set; }

        public string new_content { get; set; }

        public StarFortuneResponseDataUnit transit { get; set; }
    }

    public class StarFortuneResponseDataUnit
    {
        public string name { get; set; }

        public string value { get; set; }
    }
}
