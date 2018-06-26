using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.SyntaxChecker
{
    public interface ISyntaxChecker
    {
        bool Check(string msg, out object[] param);
    }
}