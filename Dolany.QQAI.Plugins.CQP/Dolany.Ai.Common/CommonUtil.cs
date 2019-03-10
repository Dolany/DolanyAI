using System.IO;
using System.Linq.Expressions;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Dolany.Ai.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class CommonUtil
    {
        public static Action<long, long, string, bool> MsgSendBack;

        private static readonly RNGCryptoServiceProvider RngCsp = new RNGCryptoServiceProvider();

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
            using (var r = new StreamReader($"Data/{jsonName}.json"))
            {
                var json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        public static int RandInt(int MaxValue)
        {
            const decimal _base = int.MaxValue;
            var bytes = new byte[4];
            RngCsp.GetBytes(bytes);

            return (int)(Math.Abs(BitConverter.ToInt32(bytes, 0)) / _base * MaxValue);
        }

        public static T[] RandSort<T>(T[] array)
        {
            for (var i = 0; i < array.Length; i++)
            {
                var randIdx = RandInt(array.Length - i) + i;

                array.Swap(i, randIdx);
            }

            return array;
        }

        private static void Swap<T>(this IList<T> array, int firstIdx, int secondIdx)
        {
            var temp = array[firstIdx];
            array[firstIdx] = array[secondIdx];
            array[secondIdx] = temp;
        }

        public static string ToCommonString(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

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
    }
}
