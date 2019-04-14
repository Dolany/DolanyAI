namespace Dolany.Database.Ai
{
    public class FortuneItem : DbBaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Value { get; set; }
        public int Type { get; set; }
    }
}
