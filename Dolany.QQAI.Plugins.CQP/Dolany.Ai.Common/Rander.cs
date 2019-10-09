using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Dolany.Ai.Common
{
    public static class Rander
    {
        private static readonly RNGCryptoServiceProvider RngCsp = new RNGCryptoServiceProvider();

        public static int RandInt(int MaxValue)
        {
            var bytes = new byte[4];
            RngCsp.GetBytes(bytes);

            var value = BitConverter.ToInt32(bytes, 0);
            return Math.Abs(value) % MaxValue;
        }

        public static bool RandBool()
        {
            return RandInt(2) == 0;
        }

        public static T RandElement<T>(this IEnumerable<T> collection)
        {
            var enumerable = collection.ToList();
            var length = enumerable.Count();
            var idx = RandInt(length);
            return enumerable.ElementAt(idx);
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

        public static TKey RandRated<TKey>(this Dictionary<TKey, int> ratedDic)
        {
            var list = new List<TKey>();
            foreach (var (key, value) in ratedDic)
            {
                for (var i = 0; i < value; i++)
                {
                    list.Add(key);
                }
            }

            var randArray = RandSort(list.ToArray());
            return randArray[0];
        }

        public static List<TKey> RandRated<TKey>(this Dictionary<TKey, int> ratedDic, int count)
        {
            var list = new List<TKey>();
            foreach (var (key, value) in ratedDic)
            {
                for (var i = 0; i < value; i++)
                {
                    list.Add(key);
                }
            }

            var randArray = RandSort(list.ToArray());

            var resultList = new List<TKey>();
            for (var i = 0; i < randArray.Length && resultList.Count < count; i++)
            {
                if (!resultList.Contains(randArray[i]))
                {
                    resultList.Add(randArray[i]);
                }
            }

            return resultList;
        }
    }
}
