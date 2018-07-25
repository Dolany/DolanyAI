﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolany.QQAI.Plugins.Snow.DolanyAI.Db;

namespace Dolany.QQAI.Plugins.Snow.DolanyAI
{
    [AI(
        Name = "FortuneAI",
        Description = "AI for Fortune.",
        IsAvailable = true,
        PriorityLevel = 10
        )]
    public class FortuneAI : AIBase
    {
        public FortuneAI()
            : base()
        {
            RuntimeLogger.Log("FortuneAI started.");
        }

        public override void Work()
        {
        }

        [GroupEnterCommand(
            Command = ".luck",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取每天运势",
            Syntax = "",
            Tag = "运势功能",
            SyntaxChecker = "Empty"
            )]
        public void RandomFortune(GroupMsgDTO MsgDTO, object[] param)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.RandomFortune.Where(r => r.QQNum == MsgDTO.FromQQ);
                if (!query.IsNullOrEmpty())
                {
                    var f = query.First();
                    if (DateTime.Now.Date > f.UpdateDate.Date)
                    {
                        f.FortuneValue = GetRandomFortune();
                        f.UpdateDate = DateTime.Now;
                        db.SaveChanges();
                    }

                    ShowRandFortune(MsgDTO, f);
                    return;
                }

                int randFor = GetRandomFortune();
                var rf = new RandomFortune
                {
                    Id = Guid.NewGuid().ToString(),
                    UpdateDate = DateTime.Now,
                    QQNum = MsgDTO.FromQQ,
                    FortuneValue = randFor
                };
                db.RandomFortune.Add(rf);
                ShowRandFortune(MsgDTO, rf);
            }
        }

        [GroupEnterCommand(
            Command = "星座运势",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取星座运势",
            Syntax = "[星座名]",
            Tag = "运势功能",
            SyntaxChecker = "NotEmpty"
            )]
        public void StarFortune(GroupMsgDTO MsgDTO, object[] param)
        {
            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = "查询中，请稍候"
            });
            FortuneRequestor jr = new FortuneRequestor(MsgDTO, ReportCallBack);
            Task.Run(() => jr.Work());
        }

        public void ReportCallBack(GroupMsgDTO MsgDTO, string Report)
        {
            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = Report
            });
        }

        private int GetRandomFortune()
        {
            Random rand = new Random();
            return rand.Next(101);
        }

        private void ShowRandFortune(GroupMsgDTO MsgDTO, RandomFortune rf)
        {
            string msg = "你今天的运势是：" + rf.FortuneValue + "%\r";
            for (int i = 0; i < rf.FortuneValue; i++)
            {
                msg += "|";
            }

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = msg
            });
        }
    }
}