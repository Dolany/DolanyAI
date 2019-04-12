namespace Dolany.Database.Ai
{
    public class Saying : BaseEntity
    {
        public string Cartoon { get; set; }
        public string Charactor { get; set; }
        public long FromGroup { get; set; }
        public string Content { get; set; }
    }
}
