using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolany.QQAI.Plugins.Snow.DolanyAI.Db;

namespace Dolany.QQAI.Plugins.Snow.DolanyAI
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