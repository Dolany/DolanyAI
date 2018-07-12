using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flexlive.CQP.Framework.Utils;
using AILib.Entities;
using System.ComponentModel.Composition;

namespace AILib
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

        [GroupEnterCommandAttribute(
            Command = ".luck",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取每天运势",
            Syntax = "",
            Tag = "运势功能",
            SyntaxChecker = "Empty"
            )]
        public void RandomFortune(GroupMsgDTO MsgDTO, object[] param)
        {
            var query = DbMgr.Query<RandomFortuneEntity>(r => r.QQNum == MsgDTO.FromQQ);
            if (!query.IsNullOrEmpty())
            {
                var f = query.First();
                if (string.Compare(DateTime.Now.ToDateString(), f.UpdateDate) > 0)
                {
                    f.Content = GetRandomFortune().ToString();
                    f.UpdateDate = DateTime.Now.ToDateString();
                    DbMgr.Update(f);
                }

                ShowRandFortune(MsgDTO, f);
                return;
            }

            int randFor = GetRandomFortune();
            RandomFortuneEntity rf = new RandomFortuneEntity
            {
                Id = Guid.NewGuid().ToString(),
                UpdateDate = DateTime.Now.ToDateString(),
                QQNum = MsgDTO.FromQQ,
                Content = randFor.ToString()
            };
            DbMgr.Insert(rf);
            ShowRandFortune(MsgDTO, rf);
        }

        [GroupEnterCommandAttribute(
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

        private void ShowRandFortune(GroupMsgDTO MsgDTO, RandomFortuneEntity rf)
        {
            string msg = "你今天的运势是：" + rf.Content + "%\r";
            for (int i = 0; i < int.Parse(rf.Content); i++)
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