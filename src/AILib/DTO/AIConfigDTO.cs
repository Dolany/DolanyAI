using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib
{
    public class AIConfigDTO
    {
        public long[] AimGroups { get; set; }

        public Action<long, string> SendGroupMsg { get; set; }
    }
}
