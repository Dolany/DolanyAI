using System;
using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.Model;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Ai.SingleCommand.Boardcast
{
    [AI(Name = nameof(BoardcastAI),
        IsAvailable = true,
        Description = "Ai for boardcast informations",
        PriorityLevel = 10)]
    public class BoardcastAI : AIBase
    {
        public override void Work()
        {
        }

        [EnterCommand(
            Command = "广播",
            Description = "在所有群组广播消息",
            Syntax = "广播内容",
            Tag = "系统命令",
            SyntaxChecker = "Any",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public void Board(MsgInformationEx MsgDTO, object[] param)
        {
            var content = param[0] as string;

            var info = Waiter.Instance.WaitForRelationId(new MsgCommand {Msg = AiCommand.GetGroups});
            if (info == null || string.IsNullOrEmpty(info.Msg))
            {
                return;
            }

            Console.WriteLine(info.Msg);
            RuntimeLogger.Log(info.Msg);

            var groups = ParseGroups(info.Msg);
            if (groups.IsNullOrEmpty())
            {
                return;
            }

            foreach (var group in groups)
            {
                MsgSender.Instance.PushMsg(new MsgCommand{Command = AiCommand.SendGroup, Msg = content, ToGroup = group});
            }

            MsgSender.Instance.PushMsg(MsgDTO, "广播结束！");
        }

        private List<long> ParseGroups(string groupStr)
        {
            // todo
            return null;
        }
    }
}
