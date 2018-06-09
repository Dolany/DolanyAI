using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flexlive.CQP.Framework;

namespace AILib
{
    [AI(Name = "MemoAI", Description = "AI for Memo and Alert.", IsAvailable = false)]
    public class MemoAI : AIBase
    {
        private string xmlFilePath = @"./AI/Memo/Memos.xml";

        public MemoAI(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {

        }

        public override void Work()
        {
            LoadAllMemos();
        }

        private void LoadAllMemos()
        {

        }

        [EnterCommand(Command = "备忘", SourceType = MsgType.Group)]
        private void ProccesMsg(GroupMsgDTO MsgDTO)
        {
            MemoInfo mInfo = MemoInfo.Parse(MsgDTO.msg);
            if(mInfo == null || !mInfo.IsValid)
            {
                return;
            }

            // TODO
        }
    }
}
