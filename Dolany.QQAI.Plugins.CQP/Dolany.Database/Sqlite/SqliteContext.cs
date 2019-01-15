using Dolany.Ai.Common;
using Microsoft.EntityFrameworkCore;

namespace Dolany.Database.Sqlite
{
    public class SqliteContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={CommonUtil.GetConfig("CacheDb")}");
            //optionsBuilder.UseSqlite("Data Source=Cache.db");
        }

        public DbSet<SqliteCacheModel> SqliteCacheModel { get; set; }
    }
}
