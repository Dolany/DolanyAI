namespace Dolany.Ai.Core.SyntaxChecker
{
    using System.ComponentModel.Composition;

    [Export(typeof(ISyntaxChecker))]
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

            param = new object[] { msg };
            return true;
        }
    }
}
