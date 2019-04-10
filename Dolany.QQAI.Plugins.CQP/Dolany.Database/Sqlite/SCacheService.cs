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
        private static readonly string dataSource = Configger.Instance["CacheDb"];

        public static void Cache<T>(string key, T data, DateTime expTime)
        {
            mutex.WaitOne();
            try
            {
                using (var db = new SqliteContext(dataSource))
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
            catch (Exception e)
            {
                RuntimeLogger.Log(e);
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        public static void Cache<T>(string key, T data)
        {
            var expTime = DateTime.Now.Hour < 4 ? DateTime.Today.AddHours(4) : CommonUtil.UntilTommorow().AddHours(4);
            Cache(key, data, expTime);
        }

        public static T Get<T>(string key)
        {
            mutex.WaitOne();
            using (var db = new SqliteContext(dataSource))
            {
                try
                {
                    var query = db.SqliteCacheModel.FirstOrDefault(m => m.Key == key);
                    if (query == null)
                    {
                        return default;
                    }

                    if (!string.IsNullOrEmpty(query.Value) && DateTime.TryParse(query.ExpTime, out var time) && time > DateTime.Now)
                    {
                        return JsonConvert.DeserializeObject<T>(query.Value);
                    }

                    db.SqliteCacheModel.Remove(query);
                    db.SaveChanges();

                    return default;
                }
                catch (Exception ex)
                {
                    RuntimeLogger.Log(ex);
                    return default;
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
            try
            {
                using (var db = new SqliteContext(dataSource))
                {
                    var nowStr = DateTime.Now.ToString(CultureInfo.CurrentCulture);
                    var records = db.SqliteCacheModel.Where(m => string.CompareOrdinal(nowStr, m.ExpTime) > 0);
                    db.SqliteCacheModel.RemoveRange(records);

                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                RuntimeLogger.Log(e);
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
    }
}
