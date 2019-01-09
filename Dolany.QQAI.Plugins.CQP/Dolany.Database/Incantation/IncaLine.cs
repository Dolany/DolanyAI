using System;

namespace Dolany.Database.Incantation
{
    public class IncaLine
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string CharactorName { get; set; }

        public string Line { get; set; }

        public string Scene { get; set; }
    }
}
