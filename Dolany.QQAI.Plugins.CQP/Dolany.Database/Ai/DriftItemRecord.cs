using System.Collections.Generic;

namespace Dolany.Database.Ai
{
    public class DriftItemRecord : BaseEntity
    {
        public long QQNum { get; set; }

        public IList<DriftItemCountRecord> ItemCount { get; set; }

        public IList<string> HonorList { get; set; }
    }

    public class DriftItemCountRecord
    {
        public string Name { get; set; }

        public int Count { get; set; }
    }
}
