using System.Net;
using System.Text;

namespace Dolany.Ai.Core.Net
{
    using System;

    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Model;

    using JetBrains.Annotations;

    public static class RequestHelper
    {
        [CanBeNull]
        public static ResultType PostData<ResultType>(PostReq_Param p) where ResultType : class
        {
            ResultType _reqRet = null;
            try
            {
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
            }
            catch (Exception e)
            {
                RuntimeLogger.Log(e);
            }
            return _reqRet;
        }
    }
}
