﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI
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

            var time = Utility.GenTimeFromStr(msg);
            if (time == null)
            {
                return false;
            }

            param = new object[] { time };
            return true;
        }
    }
}