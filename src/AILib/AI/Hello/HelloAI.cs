using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flexlive.CQP.Framework.Utils;
using AILib.Entities;
using Flexlive.CQP.Framework;

namespace AILib
{
    [AI(
        Name = "HelloAI",
        Description = "AI for Saying Hello to you at everyday you say at the first time in one group.",
        IsAvailable = true,
        PriorityLevel = 3
        )]
    public class HelloAI : AIBase
    {
        public HelloAI(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {
            RuntimeLogger.Log("HelloAI started.");
        }

        public override void Work()
        {
        }

        public override bool OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            if (base.OnGroupMsgReceived(MsgDTO))
            {
                return true;
            }

            var query = DbMgr.Query<HelloEntity>(h => h.GroupNum == MsgDTO.fromGroup
                    && h.QQNum == MsgDTO.fromQQ
                    && string.Compare(DateTime.Now.ToString("yyyy-MM-dd"), h.LastHelloDate) > 0
                    );
            if (query.IsNullOrEmpty())
            {
                return false;
            }
            SendHello(MsgDTO, query.First());

            return false;
        }

        private void SendHello(GroupMsgDTO MsgDTO, HelloEntity hello)
        {
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = $"{CQ.CQCode_At(MsgDTO.fromQQ)} {hello.Content}"
            });

            hello.LastHelloDate = DateTime.Now.ToString("yyyy-MM-dd");
            DbMgr.Update(hello);
        }

        [EnterCommand(
            Command = "打招呼设定",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.成员,
            Description = "设定每天打招呼的内容",
            Syntax = "",
            Tag = "打招呼功能",
            SyntaxChecker = "NotEmpty"
            )]
        public void SaveHelloContent(GroupMsgDTO MsgDTO, object[] param)
        {
            string content = param[0] as string;

            var query = DbMgr.Query<HelloEntity>(h => h.GroupNum == MsgDTO.fromGroup
                    && h.QQNum == MsgDTO.fromQQ);
            if (query.IsNullOrEmpty())
            {
                DbMgr.Insert(new HelloEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupNum = MsgDTO.fromGroup,
                    QQNum = MsgDTO.fromQQ,
                    LastHelloDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    Content = content
                });
            }
            else
            {
                var hello = query.FirstOrDefault();
                hello.Content = content;
                DbMgr.Update(hello);
            }

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = "招呼内容设定成功"
            });
        }

        [EnterCommand(
            Command = "打招呼",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.成员,
            Description = "发送打招呼的内容",
            Syntax = "",
            Tag = "打招呼功能",
            SyntaxChecker = "Empty"
            )]
        public void SayHello(GroupMsgDTO MsgDTO, object[] param)
        {
            var query = DbMgr.Query<HelloEntity>(h => h.GroupNum == MsgDTO.fromGroup
                    && h.QQNum == MsgDTO.fromQQ);
            if (query.IsNullOrEmpty())
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO
                {
                    Aim = MsgDTO.fromGroup,
                    Type = MsgType.Group,
                    Msg = "你还没有设定过招呼内容哦~"
                });
                return;
            }

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = $"{CQ.CQCode_At(MsgDTO.fromQQ)} {query.First().Content}"
            });
        }

        [EnterCommand(
            Command = "打招呼删除",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.成员,
            Description = "删除打招呼的内容",
            Syntax = "",
            Tag = "打招呼功能",
            SyntaxChecker = "Empty"
            )]
        public void DeleteHello(GroupMsgDTO MsgDTO, object[] param)
        {
            var query = DbMgr.Query<HelloEntity>(h => h.GroupNum == MsgDTO.fromGroup
                    && h.QQNum == MsgDTO.fromQQ);
            if (query.IsNullOrEmpty())
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO
                {
                    Aim = MsgDTO.fromGroup,
                    Type = MsgType.Group,
                    Msg = "你还没有设定过招呼内容哦~"
                });
                return;
            }

            DbMgr.Delete<HelloEntity>(query.First().Id);
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = "删除成功！"
            });
        }
    }
}