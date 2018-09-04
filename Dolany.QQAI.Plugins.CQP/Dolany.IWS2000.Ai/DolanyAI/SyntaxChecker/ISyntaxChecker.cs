namespace Dolany.IWS2000.Ai.DolanyAI
{
    public interface ISyntaxChecker
    {
        bool Check(string msg, out object[] param);
    }
}