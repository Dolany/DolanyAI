using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dolany.Ai.Common;
using Newtonsoft.Json;

namespace Dolany.Database.Sqlite
{
    public class SFixedSetService
    {
        private static readonly Mutex mutex = new Mutex(false, Configger<AIConfigBase>.Instance.AIConfig.FixedSetMutex);
        private static readonly string dataSource = Configger<AIConfigBase>.Instance.AIConfig.FixedSetCacheDb;

        public static void SetMaxCount(string key, int count)
        {
            mutex.WaitOne();

            try
            {
                using var db = new SqliteContext(dataSource);
                var record = db.SqliteFixedSet.FirstOrDefault(p => p.Key == key);
                if (record == null)
                {
                    record = new SqliteFixedSet() {Key = key, MaxCount = count, Value = "[]"};

                    db.SqliteFixedSet.Add(record);
                }
                else
                {
                    record.MaxCount = count;
                }

                db.SaveChanges();
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
            mutex.WaitOne();

            try
            {
                using (var db = new SqliteContext(dataSource))
                {
                    var record = db.SqliteFixedSet.FirstOrDefault(p => p.Key == key);
                    if (record == null)
                    {
                        record = new SqliteFixedSet()
                        {
                            Key = key,
                            Value = JsonConvert.SerializeObject(new List<T>(){data}),
                            MaxCount = 10
                        };

                        db.SqliteFixedSet.Add(record);
                    }
                    else
                    {
                        var list = JsonConvert.DeserializeObject<List<T>>(record.Value);
                        list = list.Prepend(data).Take(record.MaxCount).ToList();
                        record.Value = JsonConvert.SerializeObject(list);
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

        public static List<T> Get<T>(string key)
        {
            mutex.WaitOne();

            try
            {
                using (var db = new SqliteContext(dataSource))
                {
                    var record = db.SqliteFixedSet.FirstOrDefault(p => p.Key == key);
                    return record == null ? new List<T>() : JsonConvert.DeserializeObject<List<T>>(record.Value);
                }
            }
            catch (Exception e)
            {
                RuntimeLogger.Log(e);
                return new List<T>();
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
    }
}
