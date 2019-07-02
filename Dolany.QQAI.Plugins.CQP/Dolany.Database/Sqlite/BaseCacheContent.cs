using Microsoft.EntityFrameworkCore;

namespace Dolany.Database.Sqlite
{
    public class BaseCacheContent : DbContext
    {
        protected readonly string source;

        protected BaseCacheContent(string source)
        {
            this.source = source;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={source}");
        }
    }
}
