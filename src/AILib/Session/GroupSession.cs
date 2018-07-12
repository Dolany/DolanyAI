using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Session
{
    public class GroupSession
    {
        public long GroupNum { get; set; }

        public long QQNum { get; set; }

        public List<string> MsgList { get; private set; }

        public GroupSession(long GroupNum, long QQNum)
        {
            MsgList = new List<string>();
            this.GroupNum = GroupNum;
            this.QQNum = QQNum;
        }

        public void PushMsg(string msg)
        {
            MsgList.Add(msg);
        }
    }
}