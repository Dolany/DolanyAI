using System.Globalization;
using Dolany.Ai.Common;

namespace Dolany.Database.Sqlite
{
    using System;
    using System.Linq;

    using Newtonsoft.Json;

    public class SCacheService
    {
        private static readonly object Lock_obj = new object();

        public static void Cache<T>(string key, T data, DateTime expTime)
        {
            lock (Lock_obj)
            {
                using (var db = new SqliteContext())
                {
                    var query = db.SqliteCacheModel.FirstOrDefault(m => m.Key == key);
                    if (query == null)
                    {
                        db.SqliteCacheModel.Add(new SqliteCacheModel
                        {
                            Key = key, Value = JsonConvert.SerializeObject(data), ExpTime = expTime.ToString(CultureInfo.CurrentCulture)
                        });
                    }
                    else
                    {
                        query.Value = JsonConvert.SerializeObject(data);
                        query.ExpTime = expTime.ToString(CultureInfo.CurrentCulture);
                    }

                    db.SaveChanges();
                }
            }
        }

        public static void Cache<T>(string key, T data)
        {
            Cache(key, data, CommonUtil.UntilTommorow().AddHours(4));
        }

        public static T Get<T>(string key)
        {
            lock (Lock_obj)
            {
                using (var db = new SqliteContext())
                {
                    try
                    {
                        var query = db.SqliteCacheModel.FirstOrDefault(m => m.Key == key);
                        if (query == null)
                        {
                            return default(T);
                        }

                        if (!string.IsNullOrEmpty(query.Value) && DateTime.TryParse(query.ExpTime, out var time) && time > DateTime.Now)
                        {
                            return JsonConvert.DeserializeObject<T>(query.Value);
                        }

                        db.SqliteCacheModel.Remove(query);
                        db.SaveChanges();

                        return default(T);
                    }
                    catch (Exception ex)
                    {
                        RuntimeLogger.Log(ex);
                        return default(T);
                    }
                }
            }
        }

        public static void CheckOutOfDate()
        {
            lock (Lock_obj)
            {
                using (var db = new SqliteContext())
                {
                    var nowStr = DateTime.Now.ToString(CultureInfo.CurrentCulture);
                    var records = db.SqliteCacheModel.Where(m => string.CompareOrdinal(nowStr, m.ExpTime) > 0);
                    db.SqliteCacheModel.RemoveRange(records);

                    db.SaveChanges();
                }
            }
        }
    }
}
