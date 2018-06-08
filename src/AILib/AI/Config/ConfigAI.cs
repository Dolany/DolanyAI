using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib.AI.Config;

namespace AILib
{
    [AI(Name = "ConfigAI", Description = "AI for Config Other AIs.", IsAvailable = false)]
    public class ConfigAI : AIBase
    {
        public ConfigAI(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {

        }

        public override void Work()
        {
            
        }

        public override bool IsPrivateDeveloperOnly()
        {
            return true;
        }

        public override void OnPrivateMsgReceived(PrivateMsgDTO MsgDTO)
        {
            base.OnPrivateMsgReceived(MsgDTO);

            // TODO
        }
    }
}
