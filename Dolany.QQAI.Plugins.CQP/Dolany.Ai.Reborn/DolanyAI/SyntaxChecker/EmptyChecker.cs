namespace Dolany.Ai.Reborn.DolanyAI.SyntaxChecker
{
    public class EmptyChecker : ISyntaxChecker
    {
        public bool Check(string msg, out object[] param)
        {
            param = null;
            return string.IsNullOrEmpty(msg);
        }
    }
}
