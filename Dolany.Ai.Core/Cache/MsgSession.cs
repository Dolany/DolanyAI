using System.Collections.Generic;
using Dolany.Ai.Common.Models;
using Dolany.UtilityTool;

namespace Dolany.Ai.Core.Cache
{
    public class MsgSession
    {
        private long GroupNum { get; set; }
        
        private long QQNum { get; set; }
        
        private string BindAi { get; set; }
        
        private List<string> MsgList { get; set; } = new List<string>();

        public MsgSession(long GroupNum, long QQNum, string BindAi)
        {
            this.GroupNum = GroupNum;
            this.QQNum    = QQNum;
            this.BindAi   = BindAi;
        }

        public MsgSession(MsgInformation MsgDTO)
        {
            this.GroupNum = MsgDTO.FromGroup;
            this.QQNum    = MsgDTO.FromQQ;
            this.BindAi   = MsgDTO.BindAi;
        }

        public void Add(string content)
        {
            MsgList.Add(content);
        }

        public void Add(IEnumerable<string> contents)
        {
            MsgList.AddRange(contents);
        }

        public void Send()
        {
            var content = MsgList.JoinToString("\r\n");
            MsgSender.PushMsg(GroupNum, QQNum, content, BindAi);
        }
    }
}