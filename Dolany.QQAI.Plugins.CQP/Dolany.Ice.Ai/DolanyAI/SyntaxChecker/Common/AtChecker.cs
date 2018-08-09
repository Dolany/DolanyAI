using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class AtChecker : ISyntaxChecker
    {
        public bool Check(string msg, out object[] param)
        {
            param = null;
            if (!msg.StartsWith("[QQ:at="))
            {
                return false;
            }

            var strs = msg.Split(new string[] { "[QQ:at=", "]" }, StringSplitOptions.RemoveEmptyEntries);
            if (strs.IsNullOrEmpty())
            {
                return false;
            }

            long qqNum;
            if (!long.TryParse(strs[0], out qqNum))
            {
                return false;
            }

            param = new object[] { qqNum };
            return true;
        }
    }
}