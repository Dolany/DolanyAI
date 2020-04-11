namespace Dolany.Ai.Common.Models
{
    public class MsgInformation
    {
        public string Id { get; set; }
        public string Msg { get; set; }
        public string RelationId { get; set; }
        public System.DateTime? Time { get; set; }
        public long FromGroup { get; set; }
        public long FromQQ { get; set; }
        public InformationType Information { get; set; }
        public string BindAi { get; set; }

        public MsgInformationEx ToEx()
        {
            var msgEx = new MsgInformationEx
            {
                Id = Id,
                Msg = Msg,
                RelationId = RelationId,
                Time = Time,
                FromGroup = FromGroup,
                FromQQ = FromQQ,
                BindAi = BindAi
            };
            if (msgEx.FromQQ < 0)
            {
                msgEx.FromQQ &= 0xFFFFFFFF;
            }

            var msg = msgEx.Msg;
            msgEx.FullMsg = msg;
            msgEx.Command = GenCommand(ref msg);
            msgEx.Msg = msg;
            msgEx.Type = msgEx.FromGroup == 0 ? MsgType.Private : MsgType.Group;

            return msgEx;
        }

        private string GenCommand(ref string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return string.Empty;
            }

            var strs = msg.Split(' ');
            if (strs.IsNullOrEmpty())
            {
                return string.Empty;
            }

            var command = strs[0];
            msg = msg[command.Length..].Trim();
            return command;
        }
    }

    public class MsgInformationEx : MsgInformation
    {
        public string FullMsg { get; set; }
        public string Command { get; set; }
        public MsgType Type { get; set; }
        public AuthorityLevel Auth { get; set; }
        public bool IsAlive { get; set; } = true;
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
