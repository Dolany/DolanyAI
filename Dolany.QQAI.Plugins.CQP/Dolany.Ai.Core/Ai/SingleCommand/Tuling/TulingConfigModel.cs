using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Ai.SingleCommand.Tuling
{
    public class TulingConfigModel : INamedJsonModel
    {
        public string Name { get; set; }

        public string ApiKey { get; set; }
    }
}
