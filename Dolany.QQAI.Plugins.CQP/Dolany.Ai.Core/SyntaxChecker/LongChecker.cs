namespace Dolany.Ai.Core.SyntaxChecker
{
    public class LongChecker : ISyntaxChecker
    {
        public string Name { get; } = "Long";

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
