using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.Model;
using Dolany.Database;
using Dolany.Database.Ai;
using Dolany.Database.Sqlite;
using Dolany.Database.Sqlite.Model;

namespace Dolany.Ai.Core.Ai.Record.Hello
{
    [AI(Name = "打招呼",
        Description = "AI for Saying Hello to you at everyday you say at the first time in one group.",
        Enable = true,
        PriorityLevel = 15)]
    public class HelloAI : AIBase
    {
        private List<HelloRecord> HelloList = new List<HelloRecord>();

        public override void Initialization()
        {
            var Groups = Global.AllGroupsDic.Keys.ToArray();
            this.HelloList = MongoService<HelloRecord>.Get(p => Groups.Contains(p.GroupNum));
        }

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            if (MsgDTO.Type == MsgType.Private)
            {
                return false;
            }

            var key = $"Hello-{MsgDTO.FromGroup}-{MsgDTO.FromQQ}";
            var response = SCacheService.Get<HelloCache>(key);
            if (response != null)
            {
                return false;
            }

            var hello = HelloList.FirstOrDefault(h => h.GroupNum == MsgDTO.FromGroup && h.QQNum == MsgDTO.FromQQ);
            if (hello == null)
            {
                return false;
            }

            MsgSender.PushMsg(MsgDTO, $"{CodeApi.Code_At(MsgDTO.FromQQ)} {hello.Content}");
            AIAnalyzer.AddCommandCount(new CommandAnalyzeDTO()
            {
                Ai = AIAttr.Name,
                Command = "HelloOverride",
                GroupNum = MsgDTO.FromGroup,
                BindAi = MsgDTO.BindAi
            });

            var model = new HelloCache
            {
                GroupNum = MsgDTO.FromGroup,
                LastUpdateTime = DateTime.Now,
                QQNum = MsgDTO.FromQQ
            };
            SCacheService.Cache(key, model);

            return false;
        }

        [EnterCommand(ID = "HelloAI_SaveHelloContent",
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
                var hello = new HelloRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupNum = MsgDTO.FromGroup,
                    QQNum = MsgDTO.FromQQ,
                    Content = content
                };
                MongoService<HelloRecord>.Insert(hello);

                this.HelloList.Add(hello);
            }
            else
            {
                query.Content = content;
                MongoService<HelloRecord>.Update(query);
            }

            MsgSender.PushMsg(MsgDTO, "招呼内容设定成功");
            return true;
        }

        [EnterCommand(ID = "HelloAI_SayHello",
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
                MsgSender.PushMsg(MsgDTO, "你还没有设定过招呼内容哦~");
                return false;
            }

            MsgSender.PushMsg(MsgDTO, $"{CodeApi.Code_At(MsgDTO.FromQQ)} {query.Content}");
            return true;
        }

        [EnterCommand(ID = "HelloAI_DeleteHello",
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
                MsgSender.PushMsg(MsgDTO, "你还没有设定过招呼内容哦~");
                return false;
            }

            MongoService<HelloRecord>.Delete(query);
            this.HelloList.Remove(query);

            MsgSender.PushMsg(MsgDTO, "删除成功！");
            return true;
        }
    }
}