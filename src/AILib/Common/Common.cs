using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flexlive.CQP.Framework;

namespace AILib
{
    public static class Common
    {
        public static void SendMsgToDevelper(string msg)
        {
            CQ.SendPrivateMessage(1458978159, msg);
        }
    }
}
