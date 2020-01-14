using System;
using Microsoft.EntityFrameworkCore;

namespace Dolany.Database.Sqlite
{
    public class SqliteContext : BaseCacheContent, IDisposable
    {
        public DbSet<SqliteCacheModel> SqliteCacheModel { get; set; }
        public DbSet<SqliteFixedSet> SqliteFixedSet { get; set; }

        public SqliteContext(string source) : base(source)
        {
        }

        public void Dispose()
        {
        }
    }
}
