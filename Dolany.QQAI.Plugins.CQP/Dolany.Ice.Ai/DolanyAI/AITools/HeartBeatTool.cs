using System.Diagnostics;
using System.Timers;
using Dolany.Ice.Ai.DolanyAI.Utils;
using static Dolany.Ice.Ai.DolanyAI.Utils.Utility;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class HeartBeatTool : IAITool
    {
        private int CheckFrequency
        {
            get
            {
                var c = GetConfig(nameof(CheckFrequency), "10");

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

            Debug.Assert(timer != null, nameof(timer) + " != null");
            timer.Interval = CheckFrequency * 1000;
        }
    }
}