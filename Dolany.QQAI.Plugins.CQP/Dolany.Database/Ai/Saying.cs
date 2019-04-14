namespace Dolany.Database.Ai
{
    public class Saying : DbBaseEntity
    {
        public string Cartoon { get; set; }
        public string Charactor { get; set; }
        public long FromGroup { get; set; }
        public string Content { get; set; }
    }
}
