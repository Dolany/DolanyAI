using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Record.Hello
{
    public class MultiMediaHelloRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public string ContentPath { get; set; }

        public MultiMediaResourceType Type { get; set; }

        public ResourceLocationType Location { get; set; }
    }

    public enum MultiMediaResourceType
    {
        Image = 0,
        Voice = 1
    }

    public enum ResourceLocationType
    {
        LocalAbsolute = 1,
        LocalRelative = 2,
        Network = 3
    }
}
