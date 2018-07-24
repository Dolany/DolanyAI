using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class LongChecker : ISyntaxChecker
    {
        public bool Check(string msg, out object[] param)
        {
            param = null;

            if (string.IsNullOrEmpty(msg))
            {
                return false;
            }

            long memberNum;
            if (!long.TryParse(msg, out memberNum))
            {
                return false;
            }

            param = new object[] { memberNum };
            return true;
        }
    }
}