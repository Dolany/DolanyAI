using Dolany.Ice.Ai.DolanyAI.Db;

namespace Dolany.Ice.Ai.DolanyAI
{
    public static class AlertClockExtension
    {
        public static AlermClock Clone(this AlermClock ac)
        {
            var clock = new AlermClock();
            var type = clock.GetType();
            foreach (var prop in type.GetProperties())
            {
                prop.SetValue(clock, prop.GetValue(ac));
            }

            return clock;
        }
    }
}