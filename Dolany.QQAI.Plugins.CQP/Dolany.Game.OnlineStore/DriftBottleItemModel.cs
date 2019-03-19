namespace Dolany.Game.OnlineStore
{
    public class DriftBottleItemModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Rate { get; set; }

        public string Honor { get; set; }
    }

    public class DriftBottleLimitItemModel : DriftBottleItemModel
    {
        public int Year { get; set; }

        public int Month { get; set; }
    }
}
