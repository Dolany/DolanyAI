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
        public static long DeveloperNumber
        {
            get
            {
                return 1458978159;
            }
        }

        public static void SendMsgToDeveloper(string msg)
        {
            CQ.SendPrivateMessage(DeveloperNumber, msg);
        }

        public static void SendMsgToDeveloper(Exception ex)
        {
            SendMsgToDeveloper(ex.Message);
            SendMsgToDeveloper(ex.StackTrace);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> objs)
        {
            if (objs == null || objs.Count() == 0)
            {
                return true;
            }

            return false;
        }
    }
}