using System;
using System.Collections.Generic;

namespace Dolany.WorldLine.Standard.OnlineStore
{
    public class DriftBottleItemModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string PicPath { get; set; }

        public int Rate { get; set; }

        public string Honor { get; set; }

        public int Price { get; set; }

        public string[] Attributes { get; set; }

        public int Exp => Price / 10;
    }

    public class HonorModel
    {
        public string Name { get; set; }

        public List<DriftBottleItemModel> Items { get; set; }

        public virtual string FullName => Name;

        public bool IsLimit => this is LimitHonorModel;
    }

    public class LimitHonorModel : HonorModel
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public override string FullName => $"{Name}({Year}{Month:00}限定)";

        public string SortKey => $"{Year}{Month:00}";

        public bool IsCurLimit => DateTime.Now.Year == Year && DateTime.Now.Month == Month;
    }
}
