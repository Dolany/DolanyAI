namespace Dolany.Ai.Core.SyntaxChecker
{
    public class EmptyChecker : ISyntaxChecker
    {
        public string Name { get; } = "Empty";

        public bool Check(string msg, out object[] param)
        {
            param = null;
            return string.IsNullOrEmpty(msg);
        }
    }
}
