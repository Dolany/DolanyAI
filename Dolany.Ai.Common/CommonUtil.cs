using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using Newtonsoft.Json;

namespace Dolany.Ai.Common
{
    public static class CommonUtil
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> objs)
        {
            return objs == null || !objs.Any();
        }

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

        public static void Swap<T>(this IList<T> array, int firstIdx, int secondIdx)
        {
            var temp = array[firstIdx];
            array[firstIdx] = array[secondIdx];
            array[secondIdx] = temp;
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
        /// 根据表达式去除字典中的某些项目
        /// </summary>
        /// <typeparam name="TKey">Key类型</typeparam>
        /// <typeparam name="TValue">Value类型</typeparam>
        /// <param name="dic">字典</param>
        /// <param name="valueExpression">Value判定表达式</param>
        public static void Remove<TKey, TValue>(this Dictionary<TKey, TValue> dic, Expression<Predicate<TValue>> valueExpression)
        {
            if (dic == null || !dic.Any())
            {
                return;
            }

            var check = valueExpression.Compile();
            for (var i = 0; i < dic.Keys.Count; i++)
            {
                var key = dic.Keys.ElementAt(i);
                if (!check(dic[key]))
                {
                    continue;
                }

                dic.Remove(key);
                i--;
            }
        }

        public static void Remove<T>(this IList<T> list, Expression<Predicate<T>> valueExpression)
        {
            if (list.IsNullOrEmpty())
            {
                return;
            }

            var check = valueExpression.Compile();
            for (var i = 0; i < list.Count; i++)
            {
                if (!check(list[i]))
                {
                    continue;
                }

                list.RemoveAt(i);
                i--;
            }
        }

        public static T Clone<T>(this T obj) where T : class, new()
        {
            var type = obj.GetType();
            var copyT = new T();
            foreach (var prop in type.GetProperties())
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    prop.SetValue(copyT, prop.GetValue(obj));
                }
            }

            return copyT;
        }

        public static TResult Retry<TResult>(Func<TResult> RetryFunc, TimeSpan[] RetryIntervals, Predicate<TResult> ResulteChecker = null)
        {
            TResult result;
            var retryCount = 0;
            do
            {
                if (Retry(RetryFunc, out result, ResulteChecker))
                {
                    return result;
                }

                if (retryCount >= RetryIntervals.Length)
                {
                    break;
                }

                Thread.Sleep(RetryIntervals[retryCount]);
                retryCount++;
            } while (true);

            return result;
        }

        private static bool Retry<TResult>(Func<TResult> RetryFunc, out TResult Result, Predicate<TResult> ResulteChecker = null)
        {
            Result = default;
            try
            {
                Result = RetryFunc();
                return ResulteChecker == null || ResulteChecker(Result);
            }
            catch (Exception e)
            {
                RuntimeLogger.Log(e);
                return false;
            }
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
            string result;
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
            using (var stream = resp.GetResponseStream())
            {
                if (stream == null)
                {
                    return string.Empty;
                }

                //获取内容
                using var reader = new StreamReader(stream);
                result = reader.ReadToEnd();
            }

            return result;
        }

        public static string JoinToString(this IEnumerable<string> strs, string spliter)
        {
            return string.Join(spliter, strs);
        }
    }
}
