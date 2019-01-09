namespace Dolany.Database.Incantation
{
    using System;

    public class IncaCharactor
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public long QQNum { get; set; }

        public string CharactorName { get; set; }
    }
}
