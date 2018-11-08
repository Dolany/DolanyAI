using Dolany.Ai.Reborn.DolanyAI.Common;

namespace Dolany.Ai.Reborn.DolanyAI.SyntaxChecker
{
    public class HourMinuteChecker : ISyntaxChecker
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
