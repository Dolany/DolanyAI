using System;
using System.Linq;

namespace Dolany.Ai.Core.Ai.Record
{
    using Dolany.Ai.Core.Base;
    using Dolany.Ai.Core.Cache;
    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Db;
    using static Dolany.Ai.Core.API.CodeApi;

    [AI(
        Name = nameof(HelloAI),
        Description = "AI for Saying Hello to you at everyday you say at the first time in one group.",
        IsAvailable = true,
        PriorityLevel = 12)]
    public class HelloAI : AIBase
    {
        public HelloAI()
        {
            RuntimeLogger.Log("HelloAI started.");
        }

        public override void Work()
        {
        }

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            using (var db = new AIDatabase())
            {
                var now = DateTime.Now.Date;
                var query = db.Hello.Where(h => h.GroupNum == MsgDTO.FromGroup &&
                                                h.QQNum == MsgDTO.FromQQ &&
                                                now > h.LastHelloDate);
                if (query.IsNullOrEmpty())
                {
                    return false;
                }

                var hello = query.First();
                MsgSender.Instance.PushMsg(MsgDTO, $"{Code_At(MsgDTO.FromQQ)} {hello.Content}");

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
            SyntaxChecker = "Any",
            IsPrivateAvailabe = false)]
        public void SaveHelloContent(MsgInformationEx MsgDTO, object[] param)
        {
            var content = param[0] as string;

            using (var db = new AIDatabase())
            {
                var query = db.Hello.Where(h => h.GroupNum == MsgDTO.FromGroup &&
                                                h.QQNum == MsgDTO.FromQQ);
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
                    if (hello != null)
                    {
                        hello.Content = content;
                    }
                }
                db.SaveChanges();
            }

            MsgSender.Instance.PushMsg(MsgDTO, "招呼内容设定成功");
        }

        [EnterCommand(
            Command = "打招呼",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "发送打招呼的内容",
            Syntax = "",
            Tag = "打招呼功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailabe = false)]
        public void SayHello(MsgInformationEx MsgDTO, object[] param)
        {
            using (var db = new AIDatabase())
            {
                var query = db.Hello.Where(h => h.GroupNum == MsgDTO.FromGroup &&
                                                h.QQNum == MsgDTO.FromQQ);
                if (query.IsNullOrEmpty())
                {
                    MsgSender.Instance.PushMsg(MsgDTO, "你还没有设定过招呼内容哦~");
                    return;
                }

                MsgSender.Instance.PushMsg(MsgDTO, $"{Code_At(MsgDTO.FromQQ)} {query.First().Content}");
            }
        }

        [EnterCommand(
            Command = "打招呼删除",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "删除打招呼的内容",
            Syntax = "",
            Tag = "打招呼功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailabe = false)]
        public void DeleteHello(MsgInformationEx MsgDTO, object[] param)
        {
            using (var db = new AIDatabase())
            {
                var query = db.Hello.Where(h => h.GroupNum == MsgDTO.FromGroup &&
                                                h.QQNum == MsgDTO.FromQQ);
                if (query.IsNullOrEmpty())
                {
                    MsgSender.Instance.PushMsg(MsgDTO, "你还没有设定过招呼内容哦~");
                    return;
                }

                db.Hello.RemoveRange(query);
                db.SaveChanges();
            }

            MsgSender.Instance.PushMsg(MsgDTO, "删除成功！");
        }
    }
}
