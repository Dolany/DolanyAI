using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Db
{
    using Dolany.Ai.Core.Common;

    public class MsgInformationEx : MsgInformation
    {
        public string FullMsg { get; set; }
        public string Command { get; set; }
        public MsgType Type { get; set; }
    }
}
