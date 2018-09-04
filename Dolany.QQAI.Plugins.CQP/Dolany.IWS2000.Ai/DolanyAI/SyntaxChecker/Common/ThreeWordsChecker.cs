using System;

namespace Dolany.IWS2000.Ai.DolanyAI.SyntaxChecker.Common
{
    public class ThreeWordsChecker : ISyntaxChecker
    {
        public bool Check(string msg, out object[] param)
        {
            param = null;
            if (string.IsNullOrEmpty(msg))
            {
                return false;
            }

            var strs = msg.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length != 3)
            {
                return false;
            }

            param = new object[] { strs[0], strs[1], strs[2] };
            return true;
        }
    }
}