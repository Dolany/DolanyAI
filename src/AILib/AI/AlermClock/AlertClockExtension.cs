/*已迁移*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib.Db;

namespace AILib
{
    public static class AlertClockExtension
    {
        public static AlermClock Clone(this AlermClock ac)
        {
            AlermClock clock = new AlermClock();
            Type type = clock.GetType();
            foreach (var prop in type.GetProperties())
            {
                prop.SetValue(clock, prop.GetValue(ac));
            }

            return clock;
        }
    }
}