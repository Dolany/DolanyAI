using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib.Entities;

namespace AILib.SyntaxChecker
{
    public class ProcceedMsgChecker : ISyntaxChecker
    {
        public bool Check(string msg, out object[] param)
        {
            param = null;
            SayingEntity info = SayingEntity.Parse(msg);
            if (info != null)
            {
                param = new object[] { 1, info };
                return true;
            }

            string keyword = string.IsNullOrEmpty(msg.Trim()) ? null : msg;
            if (string.IsNullOrEmpty(keyword))
            {
                param = new object[] { 2 };
                return true;
            }

            param = new object[] { 3, keyword };
            return true;
        }
    }
}