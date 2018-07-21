using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.QQAI.Plugins.CQP.DolanyAI
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

            string[] strs = msg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (strs == null || strs.Length != 2)
            {
                return false;
            }

            int aimHour;
            if (!int.TryParse(strs[0], out aimHour))
            {
                return false;
            }

            long aimGroup;
            if (!long.TryParse(strs[1], out aimGroup))
            {
                return false;
            }

            param = new object[] { aimGroup, aimHour };
            return true;
        }
    }
}