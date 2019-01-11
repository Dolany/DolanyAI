namespace Dolany.Ai.Core.SyntaxChecker
{
    using Common;

    public class HourMinuteChecker : ISyntaxChecker
    {
        public string Name { get; } = "HourMinute";

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
