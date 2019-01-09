using System;

namespace Dolany.Database.Incantation
{
    public class IncaEffect
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string MagicId { get; set; }

        public int Value { get; set; }

        public string Effect { get; set; }
    }
}
