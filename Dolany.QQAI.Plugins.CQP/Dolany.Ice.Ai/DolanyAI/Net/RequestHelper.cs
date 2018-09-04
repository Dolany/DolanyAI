using System.Text;
using System.Net;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class RequestHelper
    {
        public static ResultType PostData<ResultType>(PostReq_Param p) where ResultType : class
        {
            ResultType _reqRet;
            using (var wc = new WebClient())
            {
                //post
                var postData = JsonHelper.SerializeObject(p.data);
                var bytes = Encoding.UTF8.GetBytes(postData);
                wc.Headers.Add("Content-Type", "application/json; charset=utf-8");
                wc.Headers.Add("ContentLength", postData.Length.ToString());
                var responseData = wc.UploadData($"{p.InterfaceName}", "POST", bytes);
                var re = Encoding.UTF8.GetString(responseData);
                _reqRet = JsonHelper.DeserializeJsonToObject<ResultType>(re);
            }
            return _reqRet;
        }
    }
}