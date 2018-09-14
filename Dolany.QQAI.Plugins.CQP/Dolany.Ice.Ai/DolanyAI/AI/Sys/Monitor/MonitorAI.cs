﻿using System;
using System.Linq;
using Dolany.Ice.Ai.DolanyAI.Db;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(MonitorAI),
        Description = "AI for Monitor Ais status and emitting heart beat.",
        IsAvailable = true,
        PriorityLevel = 12
        )]
    public class MonitorAI : AIBase
    {
        public override void Work()
        {
        }

        [EnterCommand(
            Command = "功能封印",
            Description = "封印一个群的某个ai功能",
            Syntax = "[群组号] [需要封印的ai名]",
            Tag = "封印功能",
            SyntaxChecker = "LongAndAny",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailabe = true
            )]
        public void SealAi(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var groupNum = (long)param[0];
            var aiName = GetAiRealName(param[1] as string);
            if (string.IsNullOrEmpty(aiName))
            {
                Utility.SendMsgToDeveloper("查找ai失败！");
                return;
            }

            using (var db = new AIDatabase())
            {
                var query = db.AISeal.Where(a => a.GroupNum == groupNum &&
                                                 a.AiName == aiName);
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
                if (attributes.Length <= 0 ||
                    !(attributes[0] is AIAttribute))
                {
                    continue;
                }
                var attr = (AIAttribute)attributes[0];
                if (attr.Name == aiName)
                {
                    return t.Name;
                }
            }

            return string.Empty;
        }
    }
}