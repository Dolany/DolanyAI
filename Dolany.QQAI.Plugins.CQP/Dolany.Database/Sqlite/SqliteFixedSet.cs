using System;

namespace Dolany.Database.Sqlite
{
    public class SqliteFixedSet
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Key { get; set; }

        public string Value { get; set; }

        public int MaxCount { get; set; }
    }
}
