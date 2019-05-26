using Microsoft.EntityFrameworkCore;

namespace Dolany.Database.Sqlite
{
    public class SqliteContext : DbContext
    {
        private readonly string source;

        public SqliteContext(string source)
        {
            this.source = source;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={source}");
        }

        public DbSet<SqliteCacheModel> SqliteCacheModel { get; set; }
        public DbSet<SqliteFixedSet> SqliteFixedSet { get; set; }
    }
}
