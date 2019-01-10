namespace Dolany.Database.Ai
{
    using System;

    public class InitInfo
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public long GroupNum { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
