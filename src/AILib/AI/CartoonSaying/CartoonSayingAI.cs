using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib.AI.CartoonSaying;

namespace AILib
{
    [AI(Name = "CartoonSayingAI", Description = "AI for Cartoon Sayings.", IsAvailable = false)]
    public class CartoonSayingAI : AIBase
    {
        public CartoonSayingAI(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {

        }

        public override void Work()
        {
            
        }

        public override void OnPrivateMsgReceived(PrivateMsgDTO MsgDTO)
        {
            base.OnPrivateMsgReceived(MsgDTO);


        }

        public override void OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            base.OnGroupMsgReceived(MsgDTO);
        }
    }
}
