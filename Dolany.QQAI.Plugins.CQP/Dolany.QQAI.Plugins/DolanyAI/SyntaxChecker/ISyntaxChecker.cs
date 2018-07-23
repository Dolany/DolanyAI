using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.QQAI.Plugins.DolanyAI
{
    public interface ISyntaxChecker
    {
        bool Check(string msg, out object[] param);
    }
}