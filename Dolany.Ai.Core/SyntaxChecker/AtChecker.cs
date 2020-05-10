using System;
using Dolany.Ai.Common;
using Dolany.UtilityTool;

namespace Dolany.Ai.Core.SyntaxChecker
{
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
