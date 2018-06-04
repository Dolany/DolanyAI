using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib
{
    [AI(Name = "HourAlertAI", Description = "AI for Hour Alert.", IsAvailable = true)]
    public class HourAlertAI : AIBase
    {
        System.Timers.Timer timer;

        public HourAlertAI(AIConfigDTO ConfigDTO)
            :base(ConfigDTO)
        {

        }

        public override void Work()
        {
            HourAlertFunc();
        }

        private void HourAlertFunc()
        {
            TimeSpan ts = GetNextHourSpan();
            timer = new System.Timers.Timer(ts.TotalMilliseconds);

            timer.AutoReset = false;
            timer.Enabled = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerUp);
        }

        private void TimerUp(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Stop();
            HourAlert(DateTime.Now.Hour.ToString());
            timer.Interval = GetNextHourSpan().TotalMilliseconds;
            timer.Start();
        }

        private TimeSpan GetNextHourSpan()
        {
            DateTime nextHour = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")).AddHours(1);
            return nextHour - DateTime.Now;
        }

        private void HourAlert(string curHour)
        {
            foreach (var groupNum in ConfigDTO.AimGroups)
            {
                ConfigDTO.SendGroupMsg(groupNum, "到" + curHour + "点啦！");
            }
        }
    }
}
