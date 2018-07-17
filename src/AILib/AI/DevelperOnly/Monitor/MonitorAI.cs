using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using AILib.Entities;
using System.ComponentModel.Composition;
using AILib.Db;

namespace AILib
{
    [AI(
        Name = "MonitorAI",
        Description = "AI for Monitor Ais status and emitting heart beat.",
        IsAvailable = false,
        PriorityLevel = 12
        )]
    public class MonitorAI : AIBase
    {
        private int CheckFrequency = 10;
        private Timer timer = new Timer();

        public MonitorAI()
            : base()
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

        [PrivateEnterCommand(
            Command = "停止心跳",
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

        [PrivateEnterCommand(
            Command = "功能封印",
            Description = "封印一个群的某个ai功能",
            Syntax = "[群组号] [需要封印的ai名]",
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

            using (AIDatabase db = new AIDatabase())
            {
                var query = db.AISeal.Where(a => a.GroupNum == groupNum && a.AiName == aiName);
                if (!query.IsNullOrEmpty())
                {
                    Common.SendMsgToDeveloper("ai功能已经在封印中！");
                    return;
                }

                AISeal aiseal = new AISeal()
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupNum = groupNum,
                    AiName = aiName
                };
                db.AISeal.Add(aiseal);
                db.SaveChanges();
            }
            Common.SendMsgToDeveloper("ai封印成功！");
        }

        private string GetAiRealName(string aiName)
        {
            var list = AIMgr.Instance.AIList;
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

        [PrivateEnterCommand(
            Command = "增加屏蔽词",
            Description = "增加需要屏蔽的词汇",
            Syntax = "[屏蔽词]",
            Tag = "屏蔽词",
            SyntaxChecker = "NotEmpty"
            )]
        public void AddDirtyWordsDic(PrivateMsgDTO MsgDTO, object[] param)
        {
            string dw = param[0] as string;
            var query = DbMgr.Query<DirtyWordEntity>(d => d.Content == dw);
            if (!query.IsNullOrEmpty())
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

        [PrivateEnterCommand(
            Command = "系统设置",
            Description = "修改系统配置",
            Syntax = "[配置项] [值]",
            Tag = "系统设置功能",
            SyntaxChecker = "TwoWords"
            )]
        public void SetConfig(PrivateMsgDTO MsgDTO, object[] param)
        {
            string configName = param[0] as string;
            string configValue = param[1] as string;

            Common.SetConfig(configName, configValue);
            Common.SendMsgToDeveloper("设置完成！");
        }
    }
}