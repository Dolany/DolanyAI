﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI
{
    public enum MsgType
    {
        Private = 1,
        Group = 0
    }

    public class SendMsgDTO
    {
        public string Guid { get; set; }

        public int SerialNum { get; set; } = 0;

        public string Msg { get; set; }

        public long Aim { get; set; }

        public MsgType Type { get; set; }
    }
}