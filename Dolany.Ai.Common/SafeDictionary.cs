using System.Collections.Generic;

namespace Dolany.Ai.Common
{
    /// <summary>
    /// 安全字典
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    public class SafeDictionary<TKey, TValue>
    {
        /// <summary>
        /// 被封装的字典数据
        /// </summary>
        public Dictionary<TKey, TValue> Data;

        public SafeDictionary(Dictionary<TKey, TValue> data)
        {
            Data = data;
        }

        /// <summary>
        /// 字典数据是否为空
        /// </summary>
        public bool IsEmpty => Data.IsNullOrEmpty();

        public TValue this[TKey key]
        {
            get => Data.GetDicValueSafe(key);
            set => Add(key, value);
        }

        /// <summary>
        /// 添加键值对到字典中，如果已经存在键，则更新其值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            if (Data == null)
            {
                Data = new Dictionary<TKey, TValue>();
            }

            if (!Data.ContainsKey(key))
            {
                Data.Add(key, value);
            }
            else
            {
                Data[key] = value;
            }
        }

        /// <summary>
        /// 根据键获取long类型的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long GetLong(TKey key)
        {
            return this[key].ToLongSafe();
        }

        /// <summary>
        /// 根据键获取int类型的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetInt(TKey key)
        {
            return this[key].ToIntSafe();
        }

        /// <summary>
        /// 根据键获取double类型的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public double GetDouble(TKey key)
        {
            return this[key].ToDoubleSafe();
        }

        /// <summary>
        /// 根据键获取string类型的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetString(TKey key)
        {
            return this[key].ToStringSafe();
        }

        public TAimType Get<TAimType>(TKey key) where TAimType: class
        {
            return this[key] as TAimType;
        }
    }
}
