using System.Collections.Generic;

namespace Dolany.Ai.Common
{
    public static class SafeConvert
    {
        public static TValue GetDicValueSafe<TKey, TValue>(this Dictionary<TKey, TValue> Dic, TKey key)
        {
            return Dic != null && Dic.ContainsKey(key) ? Dic[key] : default;
        }

        public static SafeDictionary<TKey, TValue> ToSafe<TKey, TValue>(this Dictionary<TKey, TValue> Dic)
        {
            return new SafeDictionary<TKey, TValue>(Dic);
        }

        public static string ToStringSafe(this object obj)
        {
            return obj == null ? string.Empty : obj.ToString();
        }

        public static int ToIntSafe(this object obj)
        {
            int result;
            return int.TryParse(obj.ToStringSafe(), out result) ? result : 0;
        }

        public static double ToDoubleSafe(this object obj)
        {
            double result;
            return double.TryParse(obj.ToStringSafe(), out result) ? result : 0;
        }

        public static long ToLongSafe(this object obj)
        {
            long result;
            return long.TryParse(obj.ToStringSafe(), out result) ? result : 0;
        }
    }
}
