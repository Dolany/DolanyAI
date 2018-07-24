using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI
{
    public interface ISyntaxChecker
    {
        bool Check(string msg, out object[] param);
    }
}