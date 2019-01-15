using System;
using System.Linq;
using Newtonsoft.Json;

namespace Dolany.Database.Sqlite
{
    public class SqliteCacheService
    {
        public static void Cache<T>(string key, T data, DateTime? expTime = null)
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
                        ExpTime = expTime
                    });
                }
                else
                {
                    query.Value = JsonConvert.SerializeObject(data);
                    query.ExpTime = expTime;
                }

                db.SaveChanges();
            }
        }

        public static T Get<T>(string key) where T : class
        {
            using (var db = new SqliteContext())
            {
                var query = db.SqliteCacheModel.FirstOrDefault(m => m.Key == key);
                if (query == null)
                {
                    return null;
                }

                if (!string.IsNullOrEmpty(query.Value) && !(query.ExpTime < DateTime.Now))
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
