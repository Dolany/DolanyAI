using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Ai.Game.TouhouCard
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SellCardAttribute : Attribute
    {
        public string Name { get; set; }

        public int Cost { get; set; }

        public SpellCardKind Kind { get; set; }

        public string Description { get; set; }

        public string PicPath { get; set; }
    }

    public enum SpellCardKind
    {
        对战 = 1,
        成长 = 2,
        诡计 = 3,
        财富 = 4
    }
}
