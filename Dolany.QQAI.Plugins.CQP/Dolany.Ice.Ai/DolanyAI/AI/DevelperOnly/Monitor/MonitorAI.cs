using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolany.Ice.Ai.DolanyAI.Db;
using System.Timers;
using System.Reflection;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(MonitorAI),
        Description = "AI for Monitor Ais status and emitting heart beat.",
        IsAvailable = true,
        PriorityLevel = 12
        )]
    public class MonitorAI : AIBase, IDisposable
    {
        private readonly Timer timer = new Timer();
        private readonly Timer restartTime = new Timer();

        public MonitorAI()
            : base()
        {
        }

        public int CheckFrequency
        {
            get
            {
                var c = Utility.GetConfig(nameof(CheckFrequency));
                if (string.IsNullOrEmpty(c))
                {
                    Utility.SetConfig(nameof(CheckFrequency), "10");
                    return 10;
                }

                return int.Parse(c);
            }
        }

        public override void Work()
        {
            timer.Enabled = true;
            timer.Interval = CheckFrequency * 1000;
            timer.AutoReset = false;
            timer.Elapsed += TimeUp;

            timer.Start();

            restartTime.Enabled = true;
            restartTime.Interval = (DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 03:30:30")) - DateTime.Now).TotalMilliseconds;
            restartTime.AutoReset = false;
            restartTime.Elapsed += RestartTimeUp;

            restartTime.Start();
        }

        private void RestartTimeUp(object sender, ElapsedEventArgs e)
        {
            restartTime.Stop();
            RuntimeLogger.Log("restart!");
        }

        private void TimeUp(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            Utility.SetConfig("HeartBeat", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            timer.Interval = CheckFrequency * 1000;
            timer.Start();
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
            var groupNum = (long)param[0];
            var aiName = GetAiRealName(param[1] as string);
            if (string.IsNullOrEmpty(aiName))
            {
                Utility.SendMsgToDeveloper("查找ai失败！");
                return;
            }

            using (AIDatabase db = new AIDatabase())
            {
                var query = db.AISeal.Where(a => a.GroupNum == groupNum && a.AiName == aiName);
                if (!query.IsNullOrEmpty())
                {
                    Utility.SendMsgToDeveloper("ai功能已经在封印中！");
                    return;
                }

                var aiseal = new AISeal
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupNum = groupNum,
                    AiName = aiName
                };
                db.AISeal.Add(aiseal);
                db.SaveChanges();
            }
            Utility.SendMsgToDeveloper("ai封印成功！");
        }

        private static string GetAiRealName(string aiName)
        {
            var list = AIMgr.Instance.AIList;
            foreach (var ai in list)
            {
                var t = ai.GetType();
                var attributes = t.GetCustomAttributes(typeof(AIAttribute), false);
                if (attributes.Length <= 0 || !(attributes[0] is AIAttribute))
                {
                    continue;
                }
                var attr = attributes[0] as AIAttribute;
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
            var dw = param[0] as string;
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.DirtyWord.Where(d => d.Content == dw);
                if (!query.IsNullOrEmpty())
                {
                    Utility.SendMsgToDeveloper("该词已在屏蔽列表中！");
                    return;
                }

                db.DirtyWord.Add(new DirtyWord
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = dw
                });
                db.SaveChanges();
            }
            DirtyFilter.InitWordList();
            Utility.SendMsgToDeveloper("添加成功！");
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
            var configName = param[0] as string;
            var configValue = param[1] as string;

            Utility.SetConfig(configName, configValue);
            Utility.SendMsgToDeveloper("设置完成！");
        }

        public void Dispose()
        {
            timer.Dispose();
            GC.SuppressFinalize(this);
            restartTime.Dispose();
        }
    }
}