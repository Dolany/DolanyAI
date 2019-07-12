namespace Dolany.Ai.Core.SyntaxChecker
{
    public class WordChecker : ISyntaxChecker
    {
        public string Name { get; } = "Word";

        public bool Check(string msg, out object[] param)
        {
            param = null;
            if (string.IsNullOrEmpty(msg))
            {
                return false;
            }

            if (long.TryParse(msg, out _))
            {
                return false;
            }

            param = new object[] { msg };
            return true;
        }
    }
}
