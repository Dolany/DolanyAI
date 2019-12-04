using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Ai.Sys.Version
{
    public class VersionModel : INamedJsonModel
    {
        public string Name { get; set; }

        public string VersionNum => Name;

        public string PublishDate { get; set; }

        public string[] WhatsNewList { get; set; }
    }
}
