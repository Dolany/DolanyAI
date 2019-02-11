using System.Collections.Generic;

namespace Dolany.Database.Ai
{
    public class AIEnableState : BaseEntity
    {
        public string Name { get; set; }
        public IList<long> Groups { get; set; }
    }
}
