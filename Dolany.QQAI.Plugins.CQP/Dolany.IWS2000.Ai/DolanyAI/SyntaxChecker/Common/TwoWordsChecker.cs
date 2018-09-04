using System;

namespace Dolany.IWS2000.Ai.DolanyAI
{
    public class TwoWordsChecker : ISyntaxChecker
    {
        public bool Check(string msg, out object[] param)
        {
            param = null;
            if (string.IsNullOrEmpty(msg))
            {
                return false;
            }

            var strs = msg.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length != 2)
            {
                return false;
            }

            param = new object[] { strs[0], strs[1] };
            return true;
        }
    }
}