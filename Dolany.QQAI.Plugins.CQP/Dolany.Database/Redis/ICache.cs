﻿namespace Dolany.Database.Redis
{
    using System;

    public interface ICache
    {
        /// <summary>
        /// 缓存过期时间
        /// </summary>
        int TimeOut { get; set; }

        /// <summary>
        /// 获得指定键的缓存值
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存值</returns>
        object Get(string key);

        T Get<T>(string key);

        /// <summary>
        /// 从缓存中移除指定键的缓存值
        /// </summary>
        /// <param name="key">缓存键</param>
        void Remove(string key);

        /// <summary>
        /// 将指定键的对象添加到缓存中
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        void Insert(string key, object data);

        /// <summary>
        /// 将指定键的对象添加到缓存中
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="key">
        /// 缓存键
        /// </param>
        /// <param name="data">
        /// 缓存值
        /// </param>
        void Insert<T>(string key, T data);

        /// <summary>
        /// 将指定键的对象添加到缓存中，并指定过期时间
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        /// <param name="cacheTime">缓存过期时间(秒钟)</param>
        void Insert(string key, object data, int cacheTime);

        /// <summary>
        /// 将指定键的对象添加到缓存中，并指定过期时间
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="key">
        /// 缓存键
        /// </param>
        /// <param name="data">
        /// 缓存值
        /// </param>
        /// <param name="cacheTime">
        /// 缓存过期时间(秒钟)
        /// </param>
        void Insert<T>(string key, T data, int cacheTime);

        /// <summary>
        /// 将指定键的对象添加到缓存中，并指定过期时间
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        /// <param name="cacheTime">缓存过期时间</param>
        void Insert(string key, object data, DateTime cacheTime);

        /// <summary>
        /// 将指定键的对象添加到缓存中，并指定过期时间
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="key">
        /// 缓存键
        /// </param>
        /// <param name="data">
        /// 缓存值
        /// </param>
        /// <param name="cacheTime">
        /// 缓存过期时间
        /// </param>
        void Insert<T>(string key, T data, DateTime cacheTime);

        /// <summary>
        /// 判断key是否存在
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Exists(string key);
    }
}
