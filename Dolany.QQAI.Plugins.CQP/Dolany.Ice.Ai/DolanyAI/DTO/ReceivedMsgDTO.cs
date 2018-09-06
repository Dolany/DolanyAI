namespace Dolany.Ice.Ai.DolanyAI
{
    public class ReceivedMsgDTO
    {
        public int SubType { get; set; }

        public int SendTime { get; set; }

        public long FromGroup { get; set; }

        public long FromQQ { get; set; }

        public string FromAnonymous { get; set; }

        public string Msg { get; set; }

        public int Font { get; set; }

        public string Command { get; set; }

        public string FullMsg { get; set; }

        public MsgType MsgType { get; set; }
    }
}