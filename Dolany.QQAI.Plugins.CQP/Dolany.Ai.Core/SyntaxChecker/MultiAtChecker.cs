namespace Dolany.Ai.Core.SyntaxChecker
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Dolany.Ai.Common;

    public class MultiAtChecker : ISyntaxChecker
    {
        public string Name { get; } = "MultiAt";

        public bool Check(string msg, out object[] param)
        {
            param = null;

            const string expr = @"\[QQ\:at=\d+\]";
            var mc = Regex.Matches(msg, expr);
            if (mc.IsNullOrEmpty())
            {
                return false;
            }

            var list = new List<long>();
            foreach (var m in mc)
            {
                var strs = m.ToString().Split(new[] { "[QQ:at=", "]" }, StringSplitOptions.RemoveEmptyEntries);
                if (strs.IsNullOrEmpty())
                {
                    return false;
                }

                if (!long.TryParse(strs[0], out var qqNum))
                {
                    return false;
                }

                list.Add(qqNum);
            }

            param = new object[] { list };
            return true;
        }
    }
}
