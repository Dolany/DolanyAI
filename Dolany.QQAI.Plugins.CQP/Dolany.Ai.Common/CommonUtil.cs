namespace Dolany.Ai.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class CommonUtil
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> objs)
        {
            return objs == null || !objs.Any();
        }

        public static DateTime UntilTommorow()
        {
            return DateTime.Now.AddDays(1).Date;
        }
    }
}
