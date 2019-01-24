using System.Threading;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Sys
{
    using System;
    using System.Linq;
    using Base;
    using Cache;
    using Common;
    using Model;
    using Dolany.Database.Ai;
    using Database.Sqlite.Model;
    using Database.Sqlite;

    [AI(Name = nameof(DeveloperOnlyAI),
        IsAvailable = true,
        Description = "Ai for developer only operations",
        PriorityLevel = 10)]
    public class DeveloperOnlyAI : AIBase
    {
        public override void Initialization()
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
            SCacheService.Cache(key, model);

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
            SCacheService.Cache(key, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.AddHours(hourCount));
            MsgSender.Instance.PushMsg(MsgDTO, "问卷调查模式开启");
        }

        [EnterCommand(
            Command = "权限验证关闭",
            Description = "临时关闭权限验证",
            Syntax = "持续小时数",
            Tag = "系统命令",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public void AuthDisable(MsgInformationEx MsgDTO, object[] param)
        {
            var duringHour = (long) param[0];

            const string key = "AuthDisable";
            SCacheService.Cache(key, "Disable", DateTime.Now.AddHours(duringHour));

            MsgSender.Instance.PushMsg(MsgDTO, "验证已关闭！");
        }

        [EnterCommand(
            Command = "功能奖励",
            Description = "奖励某个人某个功能若个使用次数（当日有效）",
            Syntax = "[命令名] [@QQ号] [奖励个数]",
            Tag = "系统功能",
            SyntaxChecker = "Word At Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = false)]
        public void FishingBonus(MsgInformationEx MsgDTO, object[] param)
        {
            var command = param[0] as string;
            var qqNum = (long)param[1];
            var count = (long)param[2];

            var key = $"DailyLimit-{command}-{qqNum}";
            var cache = SCacheService.Get<DailyLimitCache>(key);
            if (cache == null)
            {
                SCacheService.Cache(key, new DailyLimitCache{QQNum = qqNum, Count = (int)-count, Command = command});
            }
            else
            {
                cache.Count -= (int)count;
                SCacheService.Cache(key, cache);
            }

            MsgSender.Instance.PushMsg(MsgDTO, "奖励已生效！");
        }

        [EnterCommand(
            Command = "BlackList",
            Description = "Put someone to blacklist",
            Syntax = "qqnum",
            Tag = "系统功能",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public void BlackList(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long) param[0];
            var query = MongoService<BlackList>.Get(b => b.QQNum == qqNum).FirstOrDefault();
            if (query == null)
            {
                MongoService<BlackList>.Insert(new BlackList{QQNum = qqNum, BlackCount = 10, UpdateTime = DateTime.Now});
            }
            else
            {
                query.BlackCount = 10;
                MongoService<BlackList>.Update(query);
            }

            MsgSender.Instance.PushMsg(MsgDTO, "Success");
        }

        [EnterCommand(
            Command = "FreeBlackList",
            Description = "Pull someone out from blacklist",
            Syntax = "qqnum",
            Tag = "系统功能",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public void FreeBlackList(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long) param[0];
            var query = MongoService<BlackList>.Get(b => b.QQNum == qqNum).FirstOrDefault();
            if (query != null)
            {
                MongoService<BlackList>.Delete(query);
            }

            MsgSender.Instance.PushMsg(MsgDTO, "Success");
        }
    }
}
