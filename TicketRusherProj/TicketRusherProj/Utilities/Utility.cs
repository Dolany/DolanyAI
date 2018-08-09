using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace TicketRusherProj.Utilities
{
    public static class Utility
    {
        private static Dictionary<Type, Object> SinglonMap;
        private static string AuthCode;

        public static long DeveloperNumber
        {
            get
            {
                return 1458978159;
            }
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> objs)
        {
            if (objs == null || objs.Count() == 0)
            {
                return true;
            }

            return false;
        }

        public static string ToDateString(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }

        public static (int hour, int minute)? GenTimeFromStr(string timeStr)
        {
            string[] strs = timeStr.Split(new char[] { ':', '：' });
            if (strs == null || strs.Length != 2)
            {
                return null;
            }

            int hour;
            if (!int.TryParse(strs[0], out hour))
            {
                return null;
            }

            int minute;
            if (!int.TryParse(strs[1], out minute))
            {
                return null;
            }

            return (hour, minute);
        }

        public static T Instance<T>() where T : class, new()
        {
            if (SinglonMap == null)
            {
                SinglonMap = new Dictionary<Type, object>();
            }

            Type type = typeof(T);
            if (SinglonMap.Keys.Contains(type))
            {
                return SinglonMap[type] as T;
            }

            var value = new T();
            SinglonMap.Add(type, value);
            return value;
        }

        public static T Clone<T>(this T obj) where T : class, new()
        {
            Type type = obj.GetType();
            T copyT = new T();
            foreach (var prop in type.GetProperties())
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    prop.SetValue(copyT, prop.GetValue(obj));
                }
            }

            return copyT;
        }

        public static void SetPropertyValue(Object obj, string propName, string propValue)
        {
            Type type = obj.GetType();
            foreach (var prop in type.GetProperties())
            {
                if (prop.Name == propName)
                {
                    prop.SetValue(obj, Convert.ChangeType(propValue, prop.PropertyType));
                }
            }
        }

        public static string UrlCharConvert(string name)
        {
            string result = string.Empty;
            foreach (var c in name)
            {
                if (IsAsciiChar(c))
                {
                    result += c;
                    continue;
                }

                result += "%" + BitConverter.ToString(Encoding.UTF8.GetBytes(new char[] { c })).Replace("-", "%");
            }

            return result;
        }

        public static bool IsAsciiChar(char c)
        {
            return c >= 0x20 && c <= 0x7e;
        }

        public static T ComposePartsSelf<T>(this T obj, Assembly assembly) where T : class
        {
            var catalog = new AggregateCatalog();

            catalog.Catalogs.Add(new AssemblyCatalog(assembly));
            catalog.Catalogs.Add(new DirectoryCatalog("."));

            var _container = new CompositionContainer(catalog);

            _container.ComposeParts(obj);

            return obj;
        }
    }
}