using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class HeartBeatTool : IAITool
    {
        public int CheckFrequency
        {
            get
            {
                var c = Utility.GetConfig(nameof(CheckFrequency));
                if (string.IsNullOrEmpty(c))
                {
                    Utility.SetConfig(nameof(CheckFrequency), "10");
                    return 10;
                }

                return int.Parse(c);
            }
        }

        public void Work()
        {
            JobScheduler.Instance.Add(CheckFrequency * 1000, TimeUp);
        }

        private void TimeUp(object sender, ElapsedEventArgs e)
        {
            var timer = sender as JobTimer;
            Utility.SetConfig("HeartBeat", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            timer.Interval = CheckFrequency * 1000;
        }
    }
}