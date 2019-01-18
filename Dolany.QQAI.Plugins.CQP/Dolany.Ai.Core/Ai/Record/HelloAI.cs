using Dolany.Database.Sqlite;

namespace Dolany.Ai.Core.Ai.Record
{
    using System;
    using System.Linq;

    using Base;

    using Cache;

    using Common;

    using Dolany.Ai.Common;
    using Dolany.Database;
    using Dolany.Database.Ai;
    using Database.Sqlite;
    using Database.Sqlite.Model;

    using Model;

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

            var key = $"Hello-{MsgDTO.FromGroup}-{MsgDTO.FromQQ}";
            var response = SqliteCacheService.Get<HelloCache>(key);

            if (response != null)
            {
                return false;
            }

            var query = MongoService<Hello>.Get(h => h.GroupNum == MsgDTO.FromGroup &&
                                                     h.QQNum == MsgDTO.FromQQ);

            var hello = query.FirstOrDefault();
            if (hello == null)
            {
                return false;
            }

            MsgSender.Instance.PushMsg(MsgDTO, $"{Code_At(MsgDTO.FromQQ)} {hello.Content}");

            var model = new HelloCache
                            {
                                GroupNum = MsgDTO.FromGroup, LastUpdateTime = DateTime.Now, QQNum = MsgDTO.FromQQ
                            };
            SqliteCacheService.Cache(key, model, CommonUtil.UntilTommorow());

            return false;
        }

        [EnterCommand(
            Command = "打招呼设定",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "设定每天打招呼的内容",
            Syntax = "[设定内容]",
            Tag = "打招呼功能",
            SyntaxChecker = "Any",
            IsPrivateAvailable = false)]
        public void SaveHelloContent(MsgInformationEx MsgDTO, object[] param)
        {
            var content = param[0] as string;

            var query = MongoService<Hello>.Get(h => h.GroupNum == MsgDTO.FromGroup &&
                                                     h.QQNum == MsgDTO.FromQQ);
            if (query.IsNullOrEmpty())
            {
                MongoService<Hello>.Insert(new Hello
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
                    MongoService<Hello>.Update(hello);
                }
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
            IsPrivateAvailable = false)]
        public void SayHello(MsgInformationEx MsgDTO, object[] param)
        {
            var query = MongoService<Hello>.Get(h => h.GroupNum == MsgDTO.FromGroup &&
                                                     h.QQNum == MsgDTO.FromQQ);
            if (query.IsNullOrEmpty())
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你还没有设定过招呼内容哦~");
                return;
            }

            MsgSender.Instance.PushMsg(MsgDTO, $"{Code_At(MsgDTO.FromQQ)} {query.First().Content}");
        }

        [EnterCommand(
            Command = "打招呼删除",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "删除打招呼的内容",
            Syntax = "",
            Tag = "打招呼功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public void DeleteHello(MsgInformationEx MsgDTO, object[] param)
        {
            var query = MongoService<Hello>.Get(h => h.GroupNum == MsgDTO.FromGroup &&
                                                     h.QQNum == MsgDTO.FromQQ);
            if (query.IsNullOrEmpty())
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你还没有设定过招呼内容哦~");
                return;
            }

            MongoService<Hello>.DeleteMany(query);

            MsgSender.Instance.PushMsg(MsgDTO, "删除成功！");
        }
    }
}
