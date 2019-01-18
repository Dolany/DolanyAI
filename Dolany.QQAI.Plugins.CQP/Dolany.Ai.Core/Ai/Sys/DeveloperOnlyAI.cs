using System.Threading;

namespace Dolany.Ai.Core.Ai.Sys
{
    using System;
    using System.Linq;

    using Dolany.Ai.Common;
    using Dolany.Ai.Core.Base;
    using Dolany.Ai.Core.Cache;
    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Model;
    using Dolany.Database.Ai;
    using Database.Sqlite.Model;
    using Dolany.Database.Sqlite;

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
            var groups = Global.AllGroups;

            foreach (var group in groups)
            {
                MsgSender.Instance.PushMsg(
                    new MsgCommand { Command = AiCommand.SendGroup, Msg = content, ToGroup = group });

                Thread.Sleep(2000);
            }

            MsgSender.Instance.PushMsg(MsgDTO, "广播结束！");
        }

        [EnterCommand(
            Command = "问卷调查",
            Description = "开启问卷调查模式",
            Syntax = "持续小时数",
            Tag = "系统命令",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public void Questionnaire(MsgInformationEx MsgDTO, object[] param)
        {
            var hourCount = (long)param[0];

            const string key = "QuestionnaireDuring-QuestionnaireDuring";
            SqliteCacheService.Cache(key, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.AddHours(hourCount));
            MsgSender.Instance.PushMsg(MsgDTO, "问卷调查模式开启");
        }
    }
}
