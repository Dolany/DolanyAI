namespace Dolany.Ai.Doremi.SyntaxChecker
{
    public interface ISyntaxChecker
    {
        string Name { get; }

        bool Check(string msg, out object[] param);
    }
}
