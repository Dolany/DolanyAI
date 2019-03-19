using System.Collections.Generic;

namespace Dolany.Game.OnlineStore
{
    public class DriftBottleItemModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Rate { get; set; }

        public string Honor { get; set; }
    }

    public class HonorModel
    {
        public string Name { get; set; }

        public List<DriftBottleItemModel> Items { get; set; }

        public virtual string FullName => Name;
    }

    public class LimitHonorModel : HonorModel
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public override string FullName => $"{Name}({Year}{Month:00}限定)";
    }
}
