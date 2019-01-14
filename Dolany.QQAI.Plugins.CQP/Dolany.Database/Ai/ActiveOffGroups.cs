using System;

namespace Dolany.Database.Ai
{
    public partial class ActiveOffGroups : BaseEntity
    {
        public long GroupNum { get; set; }
        public DateTime UpdateTime { get; set; }
        public long AINum { get; set; }
    }
}
