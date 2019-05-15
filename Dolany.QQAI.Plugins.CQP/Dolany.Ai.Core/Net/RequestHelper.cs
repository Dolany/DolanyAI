using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Dolany.Ai.Core.Net
{
    using System;

    using Common;
    using Model;

    using JetBrains.Annotations;

    public static class RequestHelper
    {
        [CanBeNull]
        public static ResultType PostData<ResultType>(PostReq_Param p) where ResultType : class
        {
            ResultType _reqRet = null;
            string re = null;
            try
            {
                using (var wc = new WebClient())
                {
                    //post
                    var postData = JsonConvert.SerializeObject(p.data);
                    var bytes = Encoding.UTF8.GetBytes(postData);
                    wc.Headers.Add("Content-Type", "application/json; charset=utf-8");
                    wc.Headers.Add("ContentLength", postData.Length.ToString());
                    var responseData = wc.UploadData($"{p.InterfaceName}", "POST", bytes);
                    re = Encoding.UTF8.GetString(responseData);
                    _reqRet = JsonConvert.DeserializeObject<ResultType>(re);
                }
            }
            catch (Exception e)
            {
                Logger.Log(e);
                if (!string.IsNullOrEmpty(re))
                {
                    Logger.Log("re:" + re);
                }
            }
            return _reqRet;
        }
    }
}
