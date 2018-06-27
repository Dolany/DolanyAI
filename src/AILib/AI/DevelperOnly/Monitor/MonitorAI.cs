using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using AILib.Entities;

namespace AILib
{
    [AI(
        Name = "MonitorAI",
        Description = "AI for Monitor Ais status and emitting heart beat.",
        IsAvailable = true,
        PriorityLevel = 12
        )]
    public class MonitorAI : AIBase
    {
        private int CheckFrequency = 20;
        private Timer timer = new Timer();

        public MonitorAI(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {
            timer.Interval = CheckFrequency * 1000;
            timer.Elapsed += TimeUp;
            timer.AutoReset = true;
        }

        public override void Work()
        {
            HeartBeat();
        }

        private void HeartBeat()
        {
            timer.Enabled = true;
            timer.Start();
        }

        private void TimeUp(object sender, ElapsedEventArgs e)
        {
            var query = DbMgr.Query<HeartBeatEntity>();
            if (query.IsNullOrEmpty())
            {
                DbMgr.Insert(new HeartBeatEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    LastBeatTime = DateTime.Now,
                    Content = "HeartBeat"
                });
            }
            else
            {
                var hb = query.FirstOrDefault();
                hb.LastBeatTime = DateTime.Now;
                DbMgr.Update(hb);
            }
        }

        [EnterCommand(
            Command = "停止心跳",
            SourceType = MsgType.Private,
            IsDeveloperOnly = true,
            Description = "阻止心态",
            Syntax = "",
            Tag = "监控",
            SyntaxChecker = "Empty"
            )]
        public void StopHeart(PrivateMsgDTO MsgDTO, object[] param)
        {
            timer.Stop();

            Common.SendMsgToDeveloper("停止心跳成功！");
        }
    }
}