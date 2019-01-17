using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.Model;
using Dolany.Database.Ai;
using Dolany.Database.Redis.Model;
using Dolany.Database.Sqlite;

namespace Dolany.Ai.Core.Ai.Sys
{
    [AI(Name = nameof(DeveloperOnlyAI),
        IsAvailable = true,
        Description = "Ai for developer only operations",
        PriorityLevel = 10)]
    public class DeveloperOnlyAI : AIBase
    {
        public override void Work()
        {
        }

        [EnterCommand(
            Command = "临时授权",
            Description = "临时变更某个成员的权限等级，当日有效",
            Syntax = "[@QQ号] 权限名称",
            Tag = "系统命令",
            SyntaxChecker = "At Word",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = false)]
        public void TempAuthorize(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long)param[0];
            var authName = param[1] as string;

            var validNames = new[] { "开发者", "群主", "管理员", "成员" };
            if (!validNames.Contains(authName))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "权限名称错误！");
                return;
            }

            var key = $"TempAuthorize-{MsgDTO.FromGroup}-{qqNum}";
            var model = new TempAuthorizeCache { AuthName = authName, GroupNum = MsgDTO.FromGroup, QQNum = qqNum };
            SqliteCacheService.Cache(key, model, CommonUtil.UntilTommorow());

            MsgSender.Instance.PushMsg(MsgDTO, "临时授权成功！");
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
