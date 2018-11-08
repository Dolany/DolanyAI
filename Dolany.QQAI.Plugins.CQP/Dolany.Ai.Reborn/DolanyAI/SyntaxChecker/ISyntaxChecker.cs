namespace Dolany.Ai.Reborn.DolanyAI.SyntaxChecker
{
    public interface ISyntaxChecker
    {
        bool Check(string msg, out object[] param);
    }
}
