namespace Dolany.Ai.Reborn.DolanyAI.SyntaxChecker
{
    public class LongChecker : ISyntaxChecker
    {
        public bool Check(string msg, out object[] param)
        {
            param = null;

            if (string.IsNullOrEmpty(msg))
            {
                return false;
            }

            if (!long.TryParse(msg, out var memberNum))
            {
                return false;
            }

            param = new object[] { memberNum };
            return true;
        }
    }
}
