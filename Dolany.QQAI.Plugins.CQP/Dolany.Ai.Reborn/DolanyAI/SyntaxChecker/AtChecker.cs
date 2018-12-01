using System;
using Dolany.Ai.Reborn.DolanyAI.Common;

namespace Dolany.Ai.Reborn.DolanyAI.SyntaxChecker
{
    using System.ComponentModel.Composition;

    [Export(typeof(ISyntaxChecker))]
    public class AtChecker : ISyntaxChecker
    {
        public string Name { get; } = "At";

        public bool Check(string msg, out object[] param)
        {
            param = null;
            if (!msg.StartsWith("[QQ:at="))
            {
                return false;
            }

            var strs = msg.Split(new[] { "[QQ:at=", "]" }, StringSplitOptions.RemoveEmptyEntries);
            if (strs.IsNullOrEmpty())
            {
                return false;
            }

            if (!long.TryParse(strs[0], out var qqNum))
            {
                return false;
            }

            param = new object[] { qqNum };
            return true;
        }
    }
}
