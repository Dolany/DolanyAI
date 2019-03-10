using System.Collections.Generic;

namespace Dolany.Game.Alchemy.MagicBook.初级炼金手册
{
    public class 初级炼金手册MagicBook : IMagicBook
    {
        public override string Name { get; } = "初级炼金手册";
        public override string Description { get; } = "初级炼金师们必须掌握的基础课程";
        public override string ObtainCondition { get; } = "每个人都会的";
        public override List<IAlItem> Items { get; } = new List<IAlItem>();
    }
}
