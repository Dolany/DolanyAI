using Dolany.Ai.Common;

namespace Dolany.Ai.Doremi.Xiuxian
{
    public class ArmerModel : INamedJsonModel
    {
        public string Name { get; set; }

        public string Kind { get; set; }

        public int Value { get; set; }

        public int Price { get; set; }

        public int Rate { get; set; }
    }
}
