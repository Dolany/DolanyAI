using System;

namespace Dolany.Ai.Core.Ai.Vip
{
    public class VipServiceAttribute : Attribute
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int DiamondsNeed { get; set; }
    }
}
