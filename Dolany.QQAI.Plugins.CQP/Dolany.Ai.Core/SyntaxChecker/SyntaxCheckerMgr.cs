using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.SyntaxChecker
{
    public class SyntaxCheckerMgr
    {
        public List<ISyntaxChecker> Checkers { get; }

        public SyntaxCheckerMgr()
        {
            Checkers = CommonUtil.LoadAllInstanceFromInterface<ISyntaxChecker>();
        }

        public bool SyntaxCheck(string SyntaxChecker, string msg, out object[] param)
        {
            param = null;
            if (string.IsNullOrEmpty(SyntaxChecker))
            {
                return false;
            }

            var checkers = SyntaxChecker.Split(' ');
            var paramStrs = msg.Split(' ');

            if (checkers.Length > paramStrs.Length)
            {
                return false;
            }

            if (!checkers.Contains("Any") && checkers.Length < paramStrs.Length)
            {
                return false;
            }

            var list = new List<object>();
            for (var i = 0; i < checkers.Length; i++)
            {
                var checker = Checkers.FirstOrDefault(c => c.Name == checkers[i]);

                if (checker == null)
                {
                    return false;
                }

                if (checker.Name == "Any")
                {
                    var anyValue = string.Join(" ", paramStrs.Skip(i));
                    if (string.IsNullOrEmpty(anyValue.Trim()))
                    {
                        return false;
                    }

                    list.Add(anyValue);
                    break;
                }

                if (!checker.Check(paramStrs[i], out var p))
                {
                    return false;
                }

                if (p != null)
                {
                    list.AddRange(p);
                }
            }

            param = list.ToArray();
            return true;
        }
    }
}
