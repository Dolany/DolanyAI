using System;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class LongAndAnyChecker : ISyntaxChecker
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

            long groupNum;
            if (!long.TryParse(strs[0], out groupNum))
            {
                return false;
            }

            param = new object[] { groupNum, strs[1] };
            return true;
        }
    }
}