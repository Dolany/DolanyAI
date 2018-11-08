﻿using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace Dolany.Ai.Reborn.DolanyAI.Common
{
    public class JobTimer : Timer
    {
        public string Id { get; set; }
        public Action<object, ElapsedEventArgs> CallBack { get; set; }
        public object Data { get; set; }

        public static double WeeklyInterval => 7 * DairlyInterval;

        public static double DairlyInterval => 24 * HourlyInterval;

        public static double HourlyInterval => 60 * MinutelyInterval;

        public static double MinutelyInterval => 60 * SecondlyInterval;

        public static double SecondlyInterval => 1000;
    }

    public sealed class JobScheduler
    {
        public static JobScheduler Instance { get; } = new JobScheduler();

        private ImmutableList<JobTimer> Timers { get; } = ImmutableList.Create<JobTimer>();

        public void Stop(string Id)
        {
            var timer = Timers.First(p => p.Id == Id);
            timer?.Stop();
        }

        public void Start(string Id)
        {
            var timer = Timers.First(p => p.Id == Id);
            timer?.Start();
        }

        public void Remove(string Id)
        {
            var timer = Timers.First(p => p.Id == Id);
            if (timer == null)
            {
                return;
            }
            timer.Stop();
            Timers.Remove(timer);
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
            Debug.Assert(job != null, nameof(job) + " != null");
            job.Stop();

            job.CallBack(sender, e);

            job.Start();
        }
    }
}
