namespace Dolany.Ai.Reborn.DolanyAI.Ai.Record.AlermClock
{
    public static class AlertClockExtension
    {
        public static Db.AlermClock Clone(this Db.AlermClock ac)
        {
            var clock = new Db.AlermClock();
            var type = clock.GetType();
            foreach (var prop in type.GetProperties())
            {
                prop.SetValue(clock, prop.GetValue(ac));
            }

            return clock;
        }
    }
}
