using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace NeteaseMusicTest
{
    public class RequestHelper
    {
        public static string GetData(GetReq_Param p)
        {
            string tail = string.Empty;
            //string sig = string.Empty;//数字签名
            string _reqRet = string.Empty;
            //参数处理
            p.paramlist.ToList().ForEach(o =>
            {
                if (o.key != "privatekey")
                    tail += string.Format("{0}={1}&", o.key, o.value);
            });
            tail = tail.Substring(0, tail.Length - 1);

            using (WebClient wc = new WebClient())
            {
                Encoding enc = Encoding.GetEncoding("UTF-8");
                Byte[] pageData = wc.DownloadData(string.Format("{0}?{1}", p.InterfaceName, tail));
                _reqRet = enc.GetString(pageData);
            }
            return _reqRet;
        }
    }

    public class GetReq_Param
    {
        public string InterfaceName
        {
            get;
            set;
        }

        public IList<ReqKayValue> paramlist = new List<ReqKayValue>();
    }

    public class ReqKayValue
    {
        public string key
        {
            get;
            set;
        }

        public string value
        {
            get;
            set;
        }
    }
}