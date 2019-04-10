using System.Linq;
using System.Threading;
using Dolany.Ai.Common;

namespace Dolany.Database.Sqlite
{
    public class SFixedSetService
    {
        private static readonly Mutex mutex = new Mutex(false, Configger.Instance["Mutex"]);
        private static readonly string dataSource = Configger.Instance["CacheDb"];

        public static void SetMaxCount(string key, int count)
        {
            mutex.WaitOne();

            using (var db = new SqliteContext(dataSource))
            {
                var record = db.SqliteFixedSet.FirstOrDefault(p => p.Key == key);
                if (record == null)
                {
                    record = new SqliteFixedSet()
                    {
                        Key = key,
                        MaxCount = count,
                        Value = "[]"
                    };

                    db.SqliteFixedSet.Add(record);
                }
                else
                {
                    record.MaxCount = count;
                }

                db.SaveChanges();
            }

            mutex.ReleaseMutex();
        }

        public static void Cache<T>(string key, T data)
        {

        }
    }
}
