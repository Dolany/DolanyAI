namespace Dolany.Database.Ai
{
    public class SilenceRule : BaseEntity
    {
        public long GroupNum { get; set; }

        public string Rule { get; set; }

        public int MinValue { get; set; }

        public int MaxValue { get; set; }
    }
}
