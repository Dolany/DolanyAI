using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib
{
    public abstract class AIBase
    {
        public AIConfigDTO ConfigDTO { get; set; }

        public abstract void Work();

        public AIBase(AIConfigDTO ConfigDTO)
        {
            this.ConfigDTO = ConfigDTO;
        }

        public virtual void OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {

        }
    }
}
