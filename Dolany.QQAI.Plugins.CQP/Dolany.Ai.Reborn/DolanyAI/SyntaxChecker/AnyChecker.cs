namespace Dolany.Ai.Reborn.DolanyAI.SyntaxChecker
{
    using System.ComponentModel.Composition;

    [Export(typeof(ISyntaxChecker))]
    public class AnyChecker : ISyntaxChecker
    {
        public string Name { get; } = "Any";

        public bool Check(string msg, out object[] param)
        {
            param = new object[] { msg };
            return true;
        }
    }
}
