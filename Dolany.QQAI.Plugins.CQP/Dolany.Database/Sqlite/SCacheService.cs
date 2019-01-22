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
                                                        Key = key,
                                                        Value = JsonConvert.SerializeObject(data),
                                                        ExpTime = expTime.ToString(CultureInfo.CurrentCulture)
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
            Cache(key, data, CommonUtil.UntilTommorow());
        }

        public static T Get<T>(string key) where T : class
        {
            lock (Lock_obj)
            {
                using (var db = new SqliteContext())
                {
                    var query = db.SqliteCacheModel.FirstOrDefault(m => m.Key == key);
                    if (query == null)
                    {
                        return null;
                    }

                    if (!string.IsNullOrEmpty(query.Value) && DateTime.TryParse(query.ExpTime, out var time)
                                                           && time > DateTime.Now)
                    {
                        return JsonConvert.DeserializeObject<T>(query.Value);
                    }

                    db.SqliteCacheModel.Remove(query);
                    db.SaveChanges();

                    return null;
                }
            }
        }
    }
}
