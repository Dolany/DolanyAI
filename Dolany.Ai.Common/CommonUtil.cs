using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace Dolany.Ai.Common
{
    public static class CommonUtil
    {
        public static DateTime UntilTommorow()
        {
            return DateTime.Now.AddDays(1).Date;
        }

        public static T ReadJsonData<T>(string jsonName)
        {
            try
            {
                using var r = new StreamReader($"Data/{jsonName}.json");
                var json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return default;
            }
        }

        public static List<T> ReadJsonData_NamedList<T>(string jsonName) where T : class, INamedJsonModel
        {
            var dic = ReadJsonData<Dictionary<string, T>>(jsonName);
            if (dic == null)
            {
                return new List<T>();
            }

            foreach (var (key, value) in dic)
            {
                value.Name = key;
            }

            return dic.Values.ToList();
        }

        public static string ToCommonString(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string CurencyFormat(this int value, string mode = "Gold")
        {
            var currency = $"{value:C0}";
            var replaceSymbol = mode switch
            {
                "Gold" => Emoji.钱袋,
                "Diamond" => Emoji.钻石,
                _ => ""
            };

            return currency.Replace("¥", replaceSymbol);
        }

        /// <summary>
        /// 获取异常的详细信息
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetFullDetailMsg(this Exception ex)
        {
            if (ex == null)
            {
                return string.Empty;
            }

            var msg = ex.Message + ex.StackTrace;
            while (ex.InnerException != null)
            {
                msg += "||" + ex.InnerException.Message + ex.InnerException.StackTrace;

                ex = ex.InnerException;
            }

            return msg;
        }

        public static string GetData(string url, Dictionary<string, string> dic)
        {
            var builder = new StringBuilder();
            builder.Append(url);
            if (dic != null && dic.Count > 0)
            {
                builder.Append("?");
                var i = 0;
                foreach (var (key, value) in dic)
                {
                    if (i > 0)
                    {
                        builder.Append("&");
                    }
                    builder.AppendFormat("{0}={1}", key, HttpUtility.UrlEncode(value));
                    i++;
                }
            }
            var req = (HttpWebRequest)WebRequest.Create(builder.ToString());
            //添加参数
            var resp = (HttpWebResponse)req.GetResponse();
            using var stream = resp.GetResponseStream();

            //获取内容
            using var reader = new StreamReader(stream);
            var result = reader.ReadToEnd();

            return result;
        }

        /// <summary>
        /// 向指定接口post数据
        /// </summary>
        /// <typeparam name="ResultType">返回值反序列化类型</typeparam>
        /// <param name="interfaceName">接口地址</param>
        /// <param name="data">数据</param>
        /// <param name="timeout">超时时间(秒)</param>
        /// <returns></returns>
        public static ResultType PostData<ResultType>(string interfaceName, object data, int timeout = 100) where ResultType : class
        {
            ResultType _reqRet;
            try
            {
                var postData = JsonConvert.SerializeObject(data);
                var bytes = Encoding.UTF8.GetBytes(postData);

                var request = (HttpWebRequest) WebRequest.Create(interfaceName);
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";
                request.Timeout = timeout * 1000;
                request.ContentLength = bytes.Length;

                request.GetRequestStream().Write(bytes, 0, bytes.Length);
                using var response = (HttpWebResponse)request.GetResponse();
                using var streamReader = new StreamReader(response.GetResponseStream());
                var responseString = streamReader.ReadToEnd();
                _reqRet = JsonConvert.DeserializeObject<ResultType>(responseString);
            }
            catch (Exception)
            {
                return default;
            }

            return _reqRet;
        }
    }
}
