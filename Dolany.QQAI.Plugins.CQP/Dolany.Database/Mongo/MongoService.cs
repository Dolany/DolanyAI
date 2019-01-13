namespace Dolany.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using MongoDB.Driver;

    public class MongoService<T> where T : BaseEntity
    {
        private static MongoContext Repo { get; set; } = MongoContext.Instance;

        /// <summary>
        /// 获取集合（表）
        /// </summary>
        /// <returns></returns>
        public static IMongoCollection<T> GetCollection()
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
            return GetCollection().AsQueryable().Where(exp).ToList();
        }

        public static List<T> Get()
        {
            return Get(p => true);
        }

        /// <summary>
        /// 弱类型查询（适用分页查询）
        /// </summary>
        /// <param name="where"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetWeak(Expression<Func<T, bool>> where, int take = 0, int skip = 0)
        {
            if (take > 0)
            {
                return GetCollection().AsQueryable().Where(where).Skip(skip).Take(take).AsEnumerable();
            }
            else
            {
                return GetCollection().AsQueryable().Where(where).AsEnumerable();
            }
        }

        /// <summary>
        /// 弱类型查询 (适用排序分页查询)
        /// </summary>
        /// <typeparam name="TOrder"></typeparam>
        /// <param name="where"></param>
        /// <param name="order"></param>
        /// <param name="asc"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetWeak<TOrder>(Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order, bool asc, int take = 0, int skip = 0)
        {
            var query = GetCollection().AsQueryable().Where(where);
            query = asc ? query.OrderBy(order) : query.OrderByDescending(order);

            if (take > 0)
            {
                query = query.Skip(skip).Take(take);
            }
            return query.AsEnumerable();
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
