using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib.AI.Interaction.Publisher;

namespace AILib
{
    [AI(Name = "Publisher", Description = "AI for Publishing Developing Record.", IsAvailable = false)]
    public class Publisher : AIBase
    {
        public Publisher(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {

        }

        public override void Work()
        {

        }

        public override void OnPrivateMsgReceived(PrivateMsgDTO MsgDTO)
        {
            base.OnPrivateMsgReceived(MsgDTO);

            if(MsgDTO.fromQQ != Common.DeveloperNumber)
            {
                return;
            }

            // TODO
        }
    }
}
