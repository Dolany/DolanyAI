using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib
{
    [AI(Name = "AlermClockAI", Description = "AI for Alerm Clock.", IsAvailable = false)]
    public class AlermClockAI : AIBase
    {
        public AlermClockAI(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {

        }

        public override void Work()
        {

        }

        [EnterCommand(Command = "设置闹钟", SourceType = MsgType.Group)]
        public void SetClock(GroupMsgDTO MsgDTO)
        {

        }

        [EnterCommand(Command = "我的闹钟", SourceType = MsgType.Group)]
        public void QueryClock(GroupMsgDTO MsgDTO)
        {

        }

        [EnterCommand(Command = "删除闹钟", SourceType = MsgType.Group)]
        public void DeleteClock(GroupMsgDTO MsgDTO)
        {

        }
    }
}
