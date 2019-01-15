using System;

namespace Dolany.Database.Sqlite
{
    public class SqliteCacheModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Key { get; set; }

        public string Value { get; set; }

        public DateTime? ExpTime { get; set; }
    }
}
