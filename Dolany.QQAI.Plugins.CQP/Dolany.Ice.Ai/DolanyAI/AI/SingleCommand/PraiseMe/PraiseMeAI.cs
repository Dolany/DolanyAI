using System;
using System.Linq;
using Dolany.Ice.Ai.MahuaApis;
using Dolany.Ice.Ai.DolanyAI.Db;
using System.Threading;
using static Dolany.Ice.Ai.MahuaApis.AmandaAPIEx;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(PraiseMeAI),
        Description = "AI for Praise someone.",
        IsAvailable = true,
        PriorityLevel = 10
        )]
    public class PraiseMeAI : AIBase
    {
        private DateTime LastTime;

        private int PraiseLimit
        {
            get
            {
                var config = Utility.GetConfig(nameof(PraiseLimit), "10");

                return int.Parse(config);
            }
        }

        public PraiseMeAI()
        {
            RuntimeLogger.Log("PraiseMeAI started.");
        }

        public override void Work()
        {
            LastTime = DateTime.Now.AddMinutes(-PraiseLimit);
        }

        [GroupEnterCommand(
            Command = "赞我",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "发送十个赞",
            Syntax = "",
            Tag = "点赞功能",
            SyntaxChecker = "Empty"
            )]
        public void PraiseMe(ReceivedMsgDTO MsgDTO, object[] param)
        {
            if (!CheckLimit(MsgDTO))
            {
                return;
            }

            using (var db = new AIDatabase())
            {
                var query = db.PraiseRec.Where(p => p.QQNum == MsgDTO.FromQQ);
                if (query.IsNullOrEmpty())
                {
                    Praise(MsgDTO);
                    db.PraiseRec.Add(new PraiseRec
                    {
                        Id = Guid.NewGuid().ToString(),
                        LastDate = DateTime.Now.Date,
                        QQNum = MsgDTO.FromQQ
                    });

                    LastTime = DateTime.Now;
                }
                else if (query.First().LastDate >= DateTime.Now.Date)
                {
                    MsgSender.Instance.PushMsg(new SendMsgDTO
                    {
                        Aim = MsgDTO.FromGroup,
                        Type = MsgType.Group,
                        Msg = "今天已经赞过十次啦！"
                    });
                }
                else
                {
                    Praise(MsgDTO);
                    var praise = query.First();
                    praise.LastDate = DateTime.Now.Date;

                    LastTime = DateTime.Now;
                }
                db.SaveChanges();
            }
        }

        private bool CheckLimit(ReceivedMsgDTO MsgDTO)
        {
            if (LastTime.AddMinutes(PraiseLimit) < DateTime.Now)
            {
                return true;
            }

            var cdMinute = (LastTime.AddMinutes(PraiseLimit) - DateTime.Now).Minutes;
            var cdSecond = (LastTime.AddMinutes(PraiseLimit) - DateTime.Now).Seconds;
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = $"点赞太频繁啦！剩余冷却时间:{cdMinute}分{cdSecond}秒"
            });
            return false;
        }

        private static void Praise(ReceivedMsgDTO MsgDTO)
        {
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(100);
                SendPraise(MsgDTO.FromQQ.ToString());
            }

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Type = MsgType.Group,
                Aim = MsgDTO.FromGroup,
                Msg = $"{CodeApi.Code_At(MsgDTO.FromQQ)} 已赞十次！"
            });
        }
    }
}