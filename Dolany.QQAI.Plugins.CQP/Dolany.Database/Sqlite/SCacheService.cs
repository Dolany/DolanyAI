using System.Globalization;
using System.Threading;
using Dolany.Ai.Common;

namespace Dolany.Database.Sqlite
{
    using System;
    using System.Linq;

    using Newtonsoft.Json;

    public class SCacheService
    {
        private static readonly Mutex mutex = new Mutex(false, Configger.Instance["Mutex"]);

        public static void Cache<T>(string key, T data, DateTime expTime)
        {
            mutex.WaitOne();
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
            mutex.ReleaseMutex();
        }

        public static void Cache<T>(string key, T data)
        {
            var expTime = DateTime.Now.Hour < 4 ? DateTime.Today.AddHours(4) : CommonUtil.UntilTommorow().AddHours(4);
            Cache(key, data, expTime);
        }

        public static T Get<T>(string key)
        {
            mutex.WaitOne();
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
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }

        public static void CheckOutOfDate()
        {
            mutex.WaitOne();
            using (var db = new SqliteContext())
            {
                var nowStr = DateTime.Now.ToString(CultureInfo.CurrentCulture);
                var records = db.SqliteCacheModel.Where(m => string.CompareOrdinal(nowStr, m.ExpTime) > 0);
                db.SqliteCacheModel.RemoveRange(records);

                db.SaveChanges();
            }
            mutex.ReleaseMutex();
        }
    }
}
