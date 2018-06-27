using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib.Entities;

namespace AILib.SyntaxChecker
{
    public class HourAlertChecker : ISyntaxChecker
    {
        public bool Check(string msg, out object[] param)
        {
            param = null;
            AlertContentEntity info = AlertContentEntity.Parse(msg);
            if (info == null)
            {
                return false;
            }

            param = new object[] { info };
            return true;
        }
    }
}