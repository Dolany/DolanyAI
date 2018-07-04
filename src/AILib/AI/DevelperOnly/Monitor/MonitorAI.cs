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
            Tag = "监控功能",
            SyntaxChecker = "Empty"
            )]
        public void StopHeart(PrivateMsgDTO MsgDTO, object[] param)
        {
            timer.Stop();

            Common.SendMsgToDeveloper("停止心跳成功！");
        }

        [EnterCommand(
            Command = "功能封印",
            SourceType = MsgType.Private,
            IsDeveloperOnly = true,
            Description = "封印一个群的某个ai功能",
            Syntax = "",
            Tag = "ai封印功能",
            SyntaxChecker = "LongAndAny"
            )]
        public void SealAi(PrivateMsgDTO MsgDTO, object[] param)
        {
            long groupNum = (long)param[0];
            string aiName = GetAiRealName(param[1] as string);
            if (string.IsNullOrEmpty(aiName))
            {
                Common.SendMsgToDeveloper("查找ai失败！");
                return;
            }

            var query = DbMgr.Query<AISealEntity>(a => a.GroupNum == groupNum && a.Content == aiName);
            if (!query.IsNullOrEmpty())
            {
                Common.SendMsgToDeveloper("ai功能已经在封印中！");
                return;
            }

            AISealEntity aiseal = new AISealEntity()
            {
                Id = Guid.NewGuid().ToString(),
                GroupNum = groupNum,
                Content = aiName
            };
            DbMgr.Insert(aiseal);
            Common.SendMsgToDeveloper("ai封印成功！");
        }

        private string GetAiRealName(string aiName)
        {
            var list = AIMgr.AIList;
            foreach (var ai in list)
            {
                Type t = ai.GetType();
                object[] attributes = t.GetCustomAttributes(typeof(AIAttribute), false);
                if (attributes.Length <= 0 || !(attributes[0] is AIAttribute))
                {
                    continue;
                }
                AIAttribute attr = attributes[0] as AIAttribute;
                if (attr.Name == aiName)
                {
                    return t.Name;
                }
            }

            return string.Empty;
        }

        [EnterCommand(
            Command = "增加屏蔽词",
            SourceType = MsgType.Private,
            IsDeveloperOnly = true,
            Description = "增加需要屏蔽的词汇",
            Syntax = "",
            Tag = "屏蔽词",
            SyntaxChecker = "NotEmpty"
            )]
        public void AddDirtyWordsDic(PrivateMsgDTO MsgDTO, object[] param)
        {
            string dw = param[0] as string;
            var query = DbMgr.Query<DirtyWordEntity>(d => d.Content == dw);
            if(!query.IsNullOrEmpty())
            {
                Common.SendMsgToDeveloper("该词已在屏蔽列表中！");
                return;
            }

            DbMgr.Insert(new DirtyWordEntity
            {
                Id = Guid.NewGuid().ToString(),
                Content = dw
            });
            DirtyFilter.InitWordList();
            Common.SendMsgToDeveloper("添加成功！");
        }
    }
}