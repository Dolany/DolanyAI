using Dolany.Ai.Common;

namespace Dolany.WorldLine.Standard.Ai.SingleCommand.Tuling
{
    public class TulingConfigModel : INamedJsonModel
    {
        public string Name { get; set; }

        public string ApiKey { get; set; }
    }
}
