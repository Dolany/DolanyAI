using System;

namespace Dolany.IWS2000.Ai.DolanyAI
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

            var strs = msg.Split(new[] { "[QQ:at=", "]" }, StringSplitOptions.RemoveEmptyEntries);
            if (strs.IsNullOrEmpty())
            {
                return false;
            }

            if (!long.TryParse(strs[0], out var qqNum))
            {
                return false;
            }

            param = new object[] { qqNum };
            return true;
        }
    }
}