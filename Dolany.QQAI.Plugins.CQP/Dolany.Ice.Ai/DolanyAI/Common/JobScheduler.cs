﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class JobTimer : Timer
    {
        public string Id { get; set; }
        public Action<object, ElapsedEventArgs> CallBack { get; set; }
        public object Data { get; set; }

        public static double WeeklyInterval
        {
            get
            {
                return 7 * DairlyInterval;
            }
        }

        public static double DairlyInterval
        {
            get
            {
                return 24 * HourlyInterval;
            }
        }

        public static double HourlyInterval
        {
            get
            {
                return 60 * MinutelyInterval;
            }
        }

        public static double MinutelyInterval
        {
            get
            {
                return 60 * SecondlyInterval;
            }
        }

        public static double SecondlyInterval
        {
            get
            {
                return 1000;
            }
        }
    }

    public class JobScheduler
    {
        private static JobScheduler _instance;

        public JobScheduler()
        {
        }

        public static JobScheduler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new JobScheduler();
                }

                return _instance;
            }
        }

        public List<JobTimer> Timers { get; set; } = new List<JobTimer>();

        public void Stop(string Id)
        {
            var timer = Timers.First(p => p.Id == Id);
            if (timer != null)
            {
                timer.Stop();
            }
        }

        public void Start(string Id)
        {
            var timer = Timers.First(p => p.Id == Id);
            if (timer != null)
            {
                timer.Start();
            }
        }

        public void Remove(string Id)
        {
            var timer = Timers.First(p => p.Id == Id);
            if (timer != null)
            {
                timer.Stop();
                Timers.Remove(timer);
            }
        }

        public string Add(double Interval, Action<object, ElapsedEventArgs> CallBack, object Data = null)
        {
            var job = new JobTimer
            {
                Id = Guid.NewGuid().ToString(),
                AutoReset = false,
                Interval = Interval,
                Enabled = true,
                CallBack = CallBack,
                Data = Data
            };
            job.Elapsed += TimeUp;

            Timers.Add(job);
            job.Start();

            return job.Id;
        }

        private void TimeUp(object sender, ElapsedEventArgs e)
        {
            var job = sender as JobTimer;
            job.Stop();

            job.CallBack(sender, e);

            job.Start();
        }
    }
}