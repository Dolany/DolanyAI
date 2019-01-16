namespace Dolany.Database.Sqlite
{
    using Dolany.Ai.Common;

    using Microsoft.EntityFrameworkCore;

    public class SqliteContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={CommonUtil.GetConfig("CacheDb")}");
        }

        public DbSet<SqliteCacheModel> SqliteCacheModel { get; set; }
    }
}
