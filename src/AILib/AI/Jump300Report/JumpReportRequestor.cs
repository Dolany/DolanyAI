using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib
{
    public class JumpReportRequestor
    {
        private GroupMsgDTO MsgDTO;
        private Action<GroupMsgDTO, string> ReportCallBack;

        public JumpReportRequestor(GroupMsgDTO MsgDTO, Action<GroupMsgDTO, string> ReportCallBack)
        {
            this.MsgDTO = MsgDTO;
            this.ReportCallBack = ReportCallBack;
        }

        public void Work()
        {

        }
    }
}
