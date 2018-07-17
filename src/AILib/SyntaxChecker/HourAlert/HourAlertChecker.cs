using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib.Entities;
using AILib.Db;
using AILib;

namespace AILib.SyntaxChecker
{
    public class HourAlertChecker : ISyntaxChecker
    {
        public bool Check(string msg, out object[] param)
        {
            param = null;
            AlertContent info = AlertContentExtension.Parse(msg);
            if (info == null)
            {
                return false;
            }

            param = new object[] { info };
            return true;
        }
    }
}