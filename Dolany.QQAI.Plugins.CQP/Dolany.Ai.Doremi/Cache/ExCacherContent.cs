using System;
using Dolany.Database.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Dolany.Ai.Doremi.Cache
{
    public class ExCacherContent : BaseCacheContent
    {
        public ExCacherContent(string source) : base(source)
        {

        }

        public DbSet<PersonMsgCountRecord> PersonMsgCountRecord { get; set; }

        public DbSet<CounterEnableRecord> CounterEnableRecord { get; set; }
    }

    public class PersonMsgCountRecord
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public long QQNum { get; set; }

        public long Count { get; set; }

        public string LastDate { get; set; }

        public long? TodayCount { get; set; }
    }

    public class CounterEnableRecord
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public long QQNum { get; set; }
    }
}
