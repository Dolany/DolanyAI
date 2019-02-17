namespace Dolany.Ai.Core.Ai.Record
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Base;

    using Cache;

    using Database;
    using Dolany.Database.Ai;
    using Database.Sqlite;
    using Database.Sqlite.Model;

    using Model;

    using static API.CodeApi;

    [AI(
        Name = "打招呼",
        Description = "AI for Saying Hello to you at everyday you say at the first time in one group.",
        Enable = true,
        PriorityLevel = 15)]
    public class HelloAI : AIBase
    {
        private List<Hello> HelloList = new List<Hello>();

        public override void Initialization()
        {
            this.HelloList = MongoService<Hello>.Get();
        }

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            var key = $"Hello-{MsgDTO.FromGroup}-{MsgDTO.FromQQ}";
            var response = SCacheService.Get<HelloCache>(key);

            if (response != null)
            {
                return false;
            }

            var query = HelloList.Where(h => h.GroupNum == MsgDTO.FromGroup &&
                                                     h.QQNum == MsgDTO.FromQQ);

            var hello = query.FirstOrDefault();
            if (hello == null)
            {
                return false;
            }

            MsgSender.Instance.PushMsg(MsgDTO, $"{Code_At(MsgDTO.FromQQ)} {hello.Content}");

            var model = new HelloCache
            {
                GroupNum = MsgDTO.FromGroup,
                LastUpdateTime = DateTime.Now,
                QQNum = MsgDTO.FromQQ
            };
            SCacheService.Cache(key, model);

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
        public bool SaveHelloContent(MsgInformationEx MsgDTO, object[] param)
        {
            var content = param[0] as string;

            var query = HelloList.FirstOrDefault(h => h.GroupNum == MsgDTO.FromGroup && h.QQNum == MsgDTO.FromQQ);
            if (query == null)
            {
                var hello = new Hello
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupNum = MsgDTO.FromGroup,
                    QQNum = MsgDTO.FromQQ,
                    LastHelloDate = DateTime.Now.Date,
                    Content = content
                };
                MongoService<Hello>.Insert(hello);

                this.HelloList.Add(hello);
            }
            else
            {
                query.Content = content;
                MongoService<Hello>.Update(query);
            }

            MsgSender.Instance.PushMsg(MsgDTO, "招呼内容设定成功");
            return true;
        }

        [EnterCommand(
            Command = "打招呼",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "发送打招呼的内容",
            Syntax = "",
            Tag = "打招呼功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool SayHello(MsgInformationEx MsgDTO, object[] param)
        {
            var query = this.HelloList.FirstOrDefault(h => h.GroupNum == MsgDTO.FromGroup && h.QQNum == MsgDTO.FromQQ);
            if (query == null)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你还没有设定过招呼内容哦~");
                return false;
            }

            MsgSender.Instance.PushMsg(MsgDTO, $"{Code_At(MsgDTO.FromQQ)} {query.Content}");
            return true;
        }

        [EnterCommand(
            Command = "打招呼删除",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "删除打招呼的内容",
            Syntax = "",
            Tag = "打招呼功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool DeleteHello(MsgInformationEx MsgDTO, object[] param)
        {
            var query = HelloList.FirstOrDefault(h => h.GroupNum == MsgDTO.FromGroup && h.QQNum == MsgDTO.FromQQ);
            if (query == null)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你还没有设定过招呼内容哦~");
                return false;
            }

            MongoService<Hello>.Delete(query);
            this.HelloList.Remove(query);

            MsgSender.Instance.PushMsg(MsgDTO, "删除成功！");
            return true;
        }
    }
}