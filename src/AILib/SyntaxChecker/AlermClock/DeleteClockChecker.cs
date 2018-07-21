/*已迁移*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib;

namespace AILib.SyntaxChecker
{
    public class DeleteClockChecker : ISyntaxChecker
    {
        public bool Check(string msg, out object[] param)
        {
            param = null;

            if (string.IsNullOrEmpty(msg))
            {
                return false;
            }

            (int hour, int minute)? time = Common.GenTimeFromStr(msg);
            if (time == null)
            {
                return false;
            }

            param = new object[] { time };
            return true;
        }
    }
}