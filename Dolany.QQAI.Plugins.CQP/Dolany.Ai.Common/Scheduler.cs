using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace Dolany.Ai.Common
{
    public class SchedulerTimer : Timer
    {
        public string Id { get; set; }
        public Action<object, ElapsedEventArgs> CallBack { get; set; }
        public object Data { get; set; }
        public bool IsRepeat { get; set; } = true;

        public static double WeeklyInterval => 7 * DairlyInterval;

        public static double DairlyInterval => 24 * HourlyInterval;

        public static double HourlyInterval => 60 * MinutelyInterval;

        public static double MinutelyInterval => 60 * SecondlyInterval;

        public static double NextHourInterval =>
            (DateTime.Now.AddHours(1).AddMinutes(-DateTime.Now.Minute).AddSeconds(-DateTime.Now.Second) - DateTime.Now).TotalMilliseconds;

        public static double SecondlyInterval => 1000;
    }

    public class Scheduler
    {
        private ImmutableList<SchedulerTimer> Timers { get; } = ImmutableList.Create<SchedulerTimer>();

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
            var timer = Timers.FirstOrDefault(p => p.Id == Id);
            if (timer == null)
            {
                return;
            }
            timer.Stop();
            Timers.Remove(timer);
        }

        public string Add(double Interval,
            Action<object, ElapsedEventArgs> CallBack,
            object Data = null,
            bool IsRepeat = true,
            bool IsImmdiately = false)
        {
            var job = new SchedulerTimer
            {
                Id = Guid.NewGuid().ToString(),
                AutoReset = false,
                Interval = Interval,
                Enabled = true,
                CallBack = CallBack,
                Data = Data,
                IsRepeat = IsRepeat
            };
            job.Elapsed += TimeUp;
            if (IsImmdiately)
            {
                CallBack(job, null);
            }

            Timers.Add(job);
            job.Start();

            return job.Id;
        }

        private void TimeUp(object sender, ElapsedEventArgs e)
        {
            var job = sender as SchedulerTimer;
            Debug.Assert(job != null, nameof(job) + " != null");
            job.Stop();

            try
            {
                job.CallBack(sender, e);
            }
            catch (Exception exception)
            {
                RuntimeLogger.Log(exception);
            }
            finally
            {
                if (job.IsRepeat)
                {
                    job.Start();
                }
                else
                {
                    Remove(job.Id);
                }
            }
        }
    }
}
