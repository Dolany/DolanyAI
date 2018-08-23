namespace Dolany.Ice.Ai.DolanyAI
{
    public interface ISyntaxChecker
    {
        bool Check(string msg, out object[] param);
    }
}