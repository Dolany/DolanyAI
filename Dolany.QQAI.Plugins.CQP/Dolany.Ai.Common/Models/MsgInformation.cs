namespace Dolany.Ai.Common.Models
{
    public class MsgInformation
    {
        public string Id { get; set; }
        public string Msg { get; set; }
        public string RelationId { get; set; }
        public System.DateTime Time { get; set; }
        public long FromGroup { get; set; }
        public long FromQQ { get; set; }
        public InformationType Information { get; set; }
        public string BindAi { get; set; }
    }

    public class MsgInformationEx : MsgInformation
    {
        public string FullMsg { get; set; }
        public string Command { get; set; }
        public MsgType Type { get; set; }
        public AuthorityLevel Auth { get; set; }
    }

    public enum MsgType
    {
        Private = 1,
        Group = 0
    }

    public enum AuthorityLevel
    {
        未知 = 0,
        开发者 = 99,
        群主 = 1,
        管理员 = 2,
        成员 = 3
    }
}
