using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib
{
    [AI(Name = "HourAlertAI", Description = "AI for Hour Alert.", IsAvailable = true)]
    public class HourAlertAI : AIBase
    {
        public HourAlertAI(AIConfigDTO ConfigDTO)
            :base(ConfigDTO)
        {

        }

        public override void Work()
        {

        }
    }
}
