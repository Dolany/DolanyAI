namespace Dolany.Database.Ai
{
    public partial class MsgInformation : BaseEntity
    {
        public string Msg { get; set; }
        public string RelationId { get; set; }
        public System.DateTime Time { get; set; }
        public long FromGroup { get; set; }
        public long FromQQ { get; set; }
        public string Information { get; set; }
        public long AiNum { get; set; }
    }
}
