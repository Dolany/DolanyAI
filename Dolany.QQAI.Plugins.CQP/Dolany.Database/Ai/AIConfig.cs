namespace Dolany.Database.Ai
{
    public class AIConfig : DbBaseEntity
    {
        public string Group { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
