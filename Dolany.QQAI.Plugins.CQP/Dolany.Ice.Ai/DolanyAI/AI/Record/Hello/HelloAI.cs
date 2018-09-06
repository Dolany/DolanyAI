using System;
using System.Linq;
using Dolany.Ice.Ai.DolanyAI.Db;
using Dolany.Ice.Ai.MahuaApis;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(HelloAI),
        Description = "AI for Saying Hello to you at everyday you say at the first time in one group.",
        IsAvailable = true,
        PriorityLevel = 12
        )]
    public class HelloAI : AIBase
    {
        public HelloAI()
        {
            RuntimeLogger.Log("HelloAI started.");
        }

        public override void Work()
        {
        }

        public override bool OnMsgReceived(ReceivedMsgDTO MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            using (AIDatabase db = new AIDatabase())
            {
                var now = DateTime.Now.Date;
                var query = db.Hello.Where(h => h.GroupNum == MsgDTO.FromGroup
                    && h.QQNum == MsgDTO.FromQQ
                    && now > h.LastHelloDate
                    );
                if (query.IsNullOrEmpty())
                {
                    return false;
                }

                var hello = query.First();
                MsgSender.Instance.PushMsg(new SendMsgDTO
                {
                    Aim = MsgDTO.FromGroup,
                    Type = MsgType.Group,
                    Msg = $"{CodeApi.Code_At(MsgDTO.FromQQ)} {hello.Content}"
                });

                hello.LastHelloDate = DateTime.Now.Date;
                db.SaveChanges();
            }

            return false;
        }

        [EnterCommand(
            Command = "打招呼设定",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "设定每天打招呼的内容",
            Syntax = "[设定内容]",
            Tag = "打招呼功能",
            SyntaxChecker = "NotEmpty"
            )]
        public void SaveHelloContent(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var content = param[0] as string;

            using (AIDatabase db = new AIDatabase())
            {
                var query = db.Hello.Where(h => h.GroupNum == MsgDTO.FromGroup
                    && h.QQNum == MsgDTO.FromQQ);
                if (query.IsNullOrEmpty())
                {
                    db.Hello.Add(new Hello
                    {
                        Id = Guid.NewGuid().ToString(),
                        GroupNum = MsgDTO.FromGroup,
                        QQNum = MsgDTO.FromQQ,
                        LastHelloDate = DateTime.Now.Date,
                        Content = content
                    });
                }
                else
                {
                    var hello = query.FirstOrDefault();
                    if (hello != null) hello.Content = content;
                }
                db.SaveChanges();
            }

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = "招呼内容设定成功"
            });
        }

        [EnterCommand(
            Command = "打招呼",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "发送打招呼的内容",
            Syntax = "",
            Tag = "打招呼功能",
            SyntaxChecker = "Empty"
            )]
        public void SayHello(ReceivedMsgDTO MsgDTO, object[] param)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.Hello.Where(h => h.GroupNum == MsgDTO.FromGroup
                    && h.QQNum == MsgDTO.FromQQ);
                if (query.IsNullOrEmpty())
                {
                    MsgSender.Instance.PushMsg(new SendMsgDTO
                    {
                        Aim = MsgDTO.FromGroup,
                        Type = MsgType.Group,
                        Msg = "你还没有设定过招呼内容哦~"
                    });
                    return;
                }

                MsgSender.Instance.PushMsg(new SendMsgDTO
                {
                    Aim = MsgDTO.FromGroup,
                    Type = MsgType.Group,
                    Msg = $"{CodeApi.Code_At(MsgDTO.FromQQ)} {query.First().Content}"
                });
            }
        }

        [EnterCommand(
            Command = "打招呼删除",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "删除打招呼的内容",
            Syntax = "",
            Tag = "打招呼功能",
            SyntaxChecker = "Empty"
            )]
        public void DeleteHello(ReceivedMsgDTO MsgDTO, object[] param)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.Hello.Where(h => h.GroupNum == MsgDTO.FromGroup
                    && h.QQNum == MsgDTO.FromQQ);
                if (query.IsNullOrEmpty())
                {
                    MsgSender.Instance.PushMsg(new SendMsgDTO
                    {
                        Aim = MsgDTO.FromGroup,
                        Type = MsgType.Group,
                        Msg = "你还没有设定过招呼内容哦~"
                    });
                    return;
                }

                db.Hello.RemoveRange(query);
            }

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = "删除成功！"
            });
        }
    }
}