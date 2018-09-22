﻿namespace Dolany.Ice.Ai.DolanyAI.Utils
{
    public enum MsgType
    {
        Private = 1,
        Group = 0
    }

    public class SendMsgDTO
    {
        public string Guid { get; set; }

        public int SerialNum { get; set; }

        public string Msg { get; set; }

        public long Aim { get; set; }

        public MsgType Type { get; set; }
    }
}