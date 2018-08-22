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
        Name = nameof(SignAI),
        Description = "AI for Auto changing sign.",
        IsAvailable = true,
        PriorityLevel = 3
        )]
    public class SignAI : AIBase
    {
        public SignAI()
            : base()
        {
            RuntimeLogger.Log("SignAI started.");
        }

        public override void Work()
        {
            JobScheduler.Instance.Add(GetNextInterval(), TimeUp);
        }

        private double GetNextInterval()
        {
            var time = DateTime.Now;
            var timeS = string.Empty;
            timeS = time.Hour >= 12 ? time.AddDays(1).ToString("yyyy-MM-dd 12:05:00") : time.ToString("yyyy-MM-dd 12:05:00");

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

                var Count = signs.Count();
                var random = new Random();
                var ranIdx = random.Next(Count);

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