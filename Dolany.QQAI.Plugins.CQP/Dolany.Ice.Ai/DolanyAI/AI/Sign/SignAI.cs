using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Dolany.Ice.Ai.DolanyAI.Db;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = "SignAI",
        Description = "AI for Auto changing sign.",
        IsAvailable = true,
        PriorityLevel = 3
        )]
    public class SignAI : AIBase
    {
        private Timer timer;

        public SignAI()
            : base()
        {
            RuntimeLogger.Log("SignAI started.");
        }

        public override void Work()
        {
            timer = new Timer();
            timer.AutoReset = false;
            timer.Interval = GetNextInterval();
            timer.Enabled = true;
            timer.Elapsed += TimeUp;

            timer.Start();
        }

        private double GetNextInterval()
        {
            var time = DateTime.Now;
            var timeS = string.Empty;
            if (time.Hour >= 12)
            {
                timeS = time.AddDays(1).ToString("yyyy-MM-dd 12:05:00");
            }
            else
            {
                timeS = time.ToString("yyyy-MM-dd 12:05:00");
            }

            var aimDate = DateTime.Parse(timeS);
            return (aimDate - time).TotalMilliseconds;
        }

        private void TimeUp(object sender, ElapsedEventArgs e)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var lastTime = db.TohouSign.Where(p => p.SignTime != null).Max(p => p.SignTime);
                if (lastTime != null && (DateTime.Now - lastTime.Value).Days < 7)
                {
                    return;
                }

                var signs = db.TohouSign.Where(p => p.SignTime == null).OrderBy(p => p.Id);
                if (signs.IsNullOrEmpty())
                {
                    return;
                }

                int Count = signs.Count();
                var random = new Random();
                int ranIdx = random.Next(Count);

                var sign = signs.Skip(ranIdx).First();
                sign.SignTime = DateTime.Now;

                ChangeSign(sign.Content);

                db.SaveChanges();
            }
        }

        private void ChangeSign(string signContent)
        {
            // TODO
        }
    }
}