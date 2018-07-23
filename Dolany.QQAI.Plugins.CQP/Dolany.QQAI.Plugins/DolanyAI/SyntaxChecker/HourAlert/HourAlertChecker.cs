using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolany.QQAI.Plugins.DolanyAI.Db;

namespace Dolany.QQAI.Plugins.DolanyAI
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