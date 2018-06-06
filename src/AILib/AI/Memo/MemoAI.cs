using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flexlive.CQP.Framework;

namespace AILib
{
    [AI(Name = "MemoAI", Description = "AI for Memo and Alert.", IsAvailable = true)]
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

        public override void OnPrivateMsgReceived(PrivateMsgDTO MsgDTO)
        {
            base.OnPrivateMsgReceived(MsgDTO);

            ProccesMsg(MsgDTO.msg, MsgDTO.fromQQ);
        }

        public override void OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            base.OnGroupMsgReceived(MsgDTO);

            ProccesMsg(MsgDTO.msg, MsgDTO.fromQQ, MsgDTO.fromGroup);
        }

        private void ProccesMsg(string msg, long fromQQ, long fromGroup = 0)
        {
            MemoInfo mInfo = MemoInfo.Parse(msg);
            if(mInfo == null || !mInfo.IsValid)
            {
                return;
            }

            // TODO
        }

        private void SendMemo(MemoInfo mInfo)
        {
            if(mInfo.FromGroup == 0)
            {
                CQ.SendPrivateMessage(mInfo.Creator, mInfo.Memo);
            }
            else
            {
                CQ.SendGroupMessage(mInfo.FromGroup, $@"@{mInfo.Creator} {mInfo.Memo}");
            }
        }
    }
}
