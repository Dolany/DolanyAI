using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.QQAI.Plugins.CQP.DolanyAI
{
    public class PlusOneCache
    {
        public long GroupNumber { get; set; }

        public string MsgCache { get; set; }

        public bool IsAlreadyRepeated { get; set; }
    }
}