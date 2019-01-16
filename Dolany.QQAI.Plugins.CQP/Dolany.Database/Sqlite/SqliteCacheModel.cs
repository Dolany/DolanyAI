namespace Dolany.Database.Sqlite
{
    using System;

    public class SqliteCacheModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Key { get; set; }

        public string Value { get; set; }

        public string ExpTime { get; set; }
    }
}
