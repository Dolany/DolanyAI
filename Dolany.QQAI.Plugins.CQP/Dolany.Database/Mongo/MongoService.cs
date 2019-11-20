using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Dolany.Database
{
    public class MongoService<T> where T : DbBaseEntity
    {
        private static MongoContext Repo => MongoContext.Instance;

        /// <summary>
        /// 获取集合（表）
        /// </summary>
        /// <returns></returns>
        private static IMongoCollection<T> GetCollection()
        {
            return Repo.Collection<T>();
        }

        /// <summary>
        /// 强类型查询（适用精确查询）
        /// 查询条件包含可为空的时间类型字段时：只能查询有无值
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static List<T> Get(Expression<Func<T, bool>> exp)
        {
            return GetCollection().Find(exp).ToList();
        }

        public static List<T> Get(Expression<Func<T, bool>> findExp, Expression<Func<T, object>> sortExp, bool isAscending = true, int skip = 0, int limit = 0)
        {
            var fluent = GetCollection().Find(findExp);

            var sortBuilder = Builders<T>.Sort;
            var sortDef = isAscending ? sortBuilder.Ascending(sortExp) : sortBuilder.Descending(sortExp);
            fluent = fluent.Sort(sortDef);

            if (skip != 0)
            {
                fluent = fluent.Skip(skip);
            }

            if (limit != 0)
            {
                fluent = fluent.Limit(limit);
            }

            return fluent.ToList();
        }

        public static List<T> Get()
        {
            return Get(p => true);
        }

        public static T GetOnly(Expression<Func<T, bool>> exp)
        {
            return Get(exp).FirstOrDefault();
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="entity"></param>
        public static void Insert(T entity)
        {
            GetCollection().InsertOne(entity);
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="entitys"></param>
        public static void InsertMany(IEnumerable<T> entitys)
        {
            GetCollection().InsertMany(entitys);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        public static void Update(T entity)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
            GetCollection().ReplaceOne(filter, entity);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        public static void Delete(T entity)
        {
            GetCollection().DeleteOne(e => e.Id == entity.Id);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="where"></param>
        public static void DeleteMany(Expression<Func<T, bool>> where)
        {
            GetCollection().DeleteMany(where);
        }

        public static void DeleteMany(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }
    }
}
