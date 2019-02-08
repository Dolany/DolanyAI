namespace Dolany.Ai.Util
{
    using System;

    public class MsgInformation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Msg { get; set; }
        public string RelationId { get; set; }
        public System.DateTime Time { get; set; }
        public long FromGroup { get; set; }
        public long FromQQ { get; set; }
        public string Information { get; set; }
        public long AiNum { get; set; }
    }
}
