using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib
{
    [AI(Name = "HelperAI",
        Description = "AI for Getting Help Infos.",
        IsAvailable = false,
        PriorityLevel = 10)]
    public class HelperAI : AIBase
    {
        public HelperAI(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {
        }

        public override void Work()
        {
        }

        [EnterCommand(Command = "帮助", SourceType = MsgType.Group, AuthorityLevel = AuthorityLevel.成员)]
        public void HelpMe(GroupMsgDTO MsgDTO)
        {
        }
    }
}