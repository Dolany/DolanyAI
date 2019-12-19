using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
            using var r = new StreamReader($"Data/{jsonName}.json");
            var json = r.ReadToEnd();
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static List<T> ReadJsonData_NamedList<T>(string jsonName) where T : class, INamedJsonModel
        {
            var dic = ReadJsonData<Dictionary<string, T>>(jsonName);

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

        public static void Swap<T>(ref T obj1, ref T obj2) where T : class
        {
            var temp = obj2;
            obj2 = obj1;
            obj1 = temp;
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

        public static void AddSafe<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = value;
            }
            else
            {
                dic.Add(key, value);
            }
        }

        public static TValue GetDicValueSafe<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key)
        {
            if (dic.IsNullOrEmpty())
            {
                return default;
            }

            return dic.ContainsKey(key) ? dic[key] : default;
        }

        public static List<T> LoadAllInstanceFromClass<T>() where T : class
        {
            var assembly = Assembly.GetAssembly(typeof(T));
            var list = assembly.GetTypes()
                .Where(type => type.IsSubclassOf(typeof(T)) && !type.IsAbstract)
                .Where(type => type.FullName != null)
                .Select(type => assembly.CreateInstance(type.FullName) as T);

            return list.ToList();
        }

        public static List<T> LoadAllInstanceFromInterface<T>() where T : class
        {
            var assembly = Assembly.GetAssembly(typeof(T));
            var list = assembly.GetTypes()
                .Where(type => typeof(T).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                .Where(type => type.FullName != null)
                .Select(type => assembly.CreateInstance(type.FullName) as T);

            return list.ToList();
        }
    }
}
