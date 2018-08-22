using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class AlertPrivateChecker
    {
        public bool Check(string msg, out object[] param)
        {
            param = null;
            if (string.IsNullOrEmpty(msg))
            {
                return false;
            }

            var strs = msg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (strs == null || strs.Length != 2)
            {
                return false;
            }

            if (!int.TryParse(strs[0], out int aimHour))
            {
                return false;
            }

            if (!long.TryParse(strs[1], out long aimGroup))
            {
                return false;
            }

            param = new object[] { aimGroup, aimHour };
            return true;
        }
    }
}