using System.Collections.Generic;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.SyntaxChecker
{
    public class SyntaxCheckerMgr
    {
        public static SyntaxCheckerMgr Instance { get; } = new SyntaxCheckerMgr();

        public List<ISyntaxChecker> Checkers { get; }

        private SyntaxCheckerMgr()
        {
            Checkers = CommonUtil.LoadAllInstanceFromInterface<ISyntaxChecker>();
        }
    }
}
