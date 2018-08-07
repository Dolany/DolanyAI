using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class PrivateMsgDTO
    {
        public int SubType { get; set; }

        public int SendTime { get; set; }

        public long FromQQ { get; set; }

        public string Msg { get; set; }

        public int Font { get; set; }

        public string Command { get; set; }

        public string FullMsg { get; set; }
    }
}