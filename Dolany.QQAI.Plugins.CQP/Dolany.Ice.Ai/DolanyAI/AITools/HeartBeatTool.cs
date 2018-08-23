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
                var c = Utility.GetConfig(nameof(CheckFrequency), "10");

                return int.Parse(c);
            }
        }

        public void Work()
        {
            RuntimeLogger.Log($"{nameof(HeartBeatTool)} started.");
            JobScheduler.Instance.Add(CheckFrequency * 1000, TimeUp);
        }

        private void TimeUp(object sender, ElapsedEventArgs e)
        {
            var timer = sender as JobTimer;
            Utility.SetConfig("HeartBeat", DateTime.Now.ToCommonString());

            timer.Interval = CheckFrequency * 1000;
        }
    }
}