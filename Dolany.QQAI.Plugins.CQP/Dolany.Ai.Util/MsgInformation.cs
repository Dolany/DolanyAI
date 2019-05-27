using System;

namespace Dolany.Ai.Util
{
    public class MsgInformation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Msg { get; set; }
        public string RelationId { get; set; }
        public long FromGroup { get; set; }
        public long FromQQ { get; set; }
        public InformationType Information { get; set; }
        public string BindAi { get; set; }
    }
}
