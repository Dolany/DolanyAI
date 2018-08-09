using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using TicketRusherProj.Model;

namespace TicketRusherProj.Html
{
    public class RequestHelper
    {
        public static ResultType PostData<ResultType>(PostReq_Param p) where ResultType : class
        {
            ResultType _reqRet = default(ResultType);
            using (WebClient wc = new WebClient())
            {
                //post
                string postData = JsonHelper.SerializeObject(p.data);
                byte[] bytes = Encoding.UTF8.GetBytes(postData);
                wc.Headers.Add("Content-Type", "application/json; charset=utf-8");
                wc.Headers.Add("ContentLength", postData.Length.ToString());
                Encoding enc = Encoding.GetEncoding("UTF-8");
                byte[] responseData = wc.UploadData(string.Format("{0}", p.InterfaceName), "POST", bytes);
                string re = Encoding.UTF8.GetString(responseData);
                _reqRet = JsonHelper.DeserializeJsonToObject<ResultType>(re);
            }
            return _reqRet;
        }
    }
}