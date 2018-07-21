using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.QQAI.Plugins.CQP.DolanyAI
{
    public class SetClockChecker : ISyntaxChecker
    {
        public bool Check(string msg, out object[] param)
        {
            param = null;
            string[] strs = msg.Split(new char[] { ' ' });
            if (strs == null || strs.Length < 2)
            {
                return false;
            }

            (int hour, int minute)? time = Utility.GenTimeFromStr(strs[0]);
            if (time == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(strs[1]))
            {
                return false;
            }

            param = new object[] { time, strs[1] };
            return true;
        }
    }
}