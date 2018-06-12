using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib.Entities;

namespace AILib
{
    [AI(Name = "RandomPicAI", Description = "AI for Sending Random Pic By Keyword.", IsAvailable = false)]
    public class RandomPicAI : AIBase
    {
        public RandomPicAI(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {

        }

        public override void Work()
        {
            
        }

        public override void OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            
        }
    }
}
