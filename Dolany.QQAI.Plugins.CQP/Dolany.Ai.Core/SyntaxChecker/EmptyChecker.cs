namespace Dolany.Ai.Core.SyntaxChecker
{
    using System.ComponentModel.Composition;

    [Export(typeof(ISyntaxChecker))]
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
