using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolany.Ice.Ai.MahuaApis;
using Dolany.Ice.Ai.DolanyAI.Db;
using System.Threading;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = "PraiseMeAI",
        Description = "AI for Praise someone.",
        IsAvailable = true,
        PriorityLevel = 10
        )]
    public class PraiseMeAI : AIBase
    {
        private DateTime LastTime = DateTime.Now;

        private int PraiseLimit
        {
            get
            {
                var config = Utility.GetConfig("PraiseLimit");
                if (string.IsNullOrEmpty(config))
                {
                    Utility.SetConfig("PraiseLimit", "10");
                    return 10;
                }

                return int.Parse(config);
            }
        }

        public PraiseMeAI()
            : base()
        {
            RuntimeLogger.Log("PraiseMeAI started.");
        }

        public override void Work()
        {
        }

        [GroupEnterCommand(
            Command = "赞我",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "发送十个赞",
            Syntax = "",
            Tag = "点赞功能",
            SyntaxChecker = "Empty"
            )]
        public void PraiseMe(GroupMsgDTO MsgDTO, object[] param)
        {
            if (LastTime.AddMinutes(PraiseLimit) >= DateTime.Now)
            {
                int cdMinute = (LastTime.AddMinutes(PraiseLimit) - DateTime.Now).Minutes;
                int cdSecond = (LastTime.AddMinutes(PraiseLimit) - DateTime.Now).Seconds;
                MsgSender.Instance.PushMsg(new SendMsgDTO
                {
                    Aim = MsgDTO.FromGroup,
                    Type = MsgType.Group,
                    Msg = $"点赞太频繁啦！剩余冷却时间:{cdMinute}分{cdSecond}秒"
                });
                return;
            }

            using (AIDatabase db = new AIDatabase())
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

        private void Praise(GroupMsgDTO MsgDTO)
        {
            int result = -1;
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(100);
                result = AmandaAPIEx.SendPraise(MsgDTO.FromQQ.ToString());
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