namespace Dolany.Ai.Core.Db
{
    public partial class MsgInformation
    {
        public string Id { get; set; }
        public string Msg { get; set; }
        public string RelationId { get; set; }
        public System.DateTime Time { get; set; }
        public long FromGroup { get; set; }
        public long FromQQ { get; set; }
    }
}
