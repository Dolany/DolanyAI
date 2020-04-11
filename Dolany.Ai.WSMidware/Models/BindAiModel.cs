using Dolany.Ai.Common;

namespace Dolany.Ai.WSMidware.Models
{
    public class BindAiModel : INamedJsonModel
    {
        public string Name { get; set; }

        public string BindPort { get; set; }

        public string QQNum { get; set; }
    }
}
