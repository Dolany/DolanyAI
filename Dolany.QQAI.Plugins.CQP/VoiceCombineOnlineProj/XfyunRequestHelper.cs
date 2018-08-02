using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Dolany.Ice.Ai.MahuaApis;
using Dolany.Ice.Ai.DolanyAI;

namespace VoiceCombineOnlineProj
{
    public class XfyunRequestHelper
    {
        public static string PostData(PostReq_Param p)
        {
            using (WebClient wc = new WebClient())
            {
                string appid = "5b62c9d3";
                string CurTime = GetUtcLong().ToString();
                string Params = null;
                string CheckSum = null;
                //post
                string postData = JsonHelper.SerializeObject(p.data);
                byte[] bytes = Encoding.UTF8.GetBytes(postData);
                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded; charset=utf-8");
                //wc.Headers.Add("ContentLength", postData.Length.ToString());
                wc.Headers.Add("X-Appid", appid);
                wc.Headers.Add("X-CurTime", CurTime);
                wc.Headers.Add("X-Param", Params);
                wc.Headers.Add("X-CheckSum", CheckSum);
                Encoding enc = Encoding.GetEncoding("UTF-8");
                byte[] responseData = wc.UploadData(string.Format("{0}", p.InterfaceName), "POST", bytes);
                string re = Encoding.UTF8.GetString(responseData);
                return re;
                //_reqRet = JsonHelper.DeserializeJsonToObject<ResultType>(re);
            }
            //return _reqRet;
        }

        private static long GetUtcLong()
        {
            DateTime UtcTime = new DateTime(1970, 1, 1, 0, 0, 0);
            var span = DateTime.Now.ToUniversalTime() - UtcTime;
            return (long)span.TotalSeconds;
        }
    }
}