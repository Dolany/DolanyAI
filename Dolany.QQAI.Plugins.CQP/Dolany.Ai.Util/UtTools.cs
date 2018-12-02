using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ai.Util
{
    public static class UtTools
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> objs)
        {
            return objs == null || !objs.Any();
        }
    }
}
