using System.Threading;
using Dolany.Ai.Common;
using Dolany.Database;
using Dolany.Game.OnlineStore;

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

    [AI(Name = "开发者后台",
        Enable = true,
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
        public bool TempAuthorize(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long)param[0];
            var authName = param[1] as string;

            var validNames = new[] { "开发者", "群主", "管理员", "成员" };
            if (!validNames.Contains(authName))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "权限名称错误！");
                return false;
            }

            var key = $"TempAuthorize-{MsgDTO.FromGroup}-{qqNum}";
            var model = new TempAuthorizeCache { AuthName = authName, GroupNum = MsgDTO.FromGroup, QQNum = qqNum };
            SCacheService.Cache(key, model);

            MsgSender.Instance.PushMsg(MsgDTO, "临时授权成功！");
            return true;
        }

        [EnterCommand(
            Command = "广播",
            Description = "在所有群组广播消息",
            Syntax = "广播内容",
            Tag = "系统命令",
            SyntaxChecker = "Any",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool Board(MsgInformationEx MsgDTO, object[] param)
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
            return true;
        }

        [EnterCommand(
            Command = "问卷调查",
            Description = "开启问卷调查模式",
            Syntax = "持续小时数",
            Tag = "系统命令",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool Questionnaire(MsgInformationEx MsgDTO, object[] param)
        {
            var hourCount = (long)param[0];

            const string key = "QuestionnaireDuring-QuestionnaireDuring";
            SCacheService.Cache(key, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.AddHours(hourCount));
            MsgSender.Instance.PushMsg(MsgDTO, "问卷调查模式开启");

            return true;
        }

        [EnterCommand(
            Command = "权限验证关闭",
            Description = "临时关闭权限验证",
            Syntax = "持续小时数",
            Tag = "系统命令",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool AuthDisable(MsgInformationEx MsgDTO, object[] param)
        {
            var duringHour = (long) param[0];

            const string key = "AuthDisable";
            SCacheService.Cache(key, "Disable", DateTime.Now.AddHours(duringHour));

            MsgSender.Instance.PushMsg(MsgDTO, "验证已关闭！");
            return true;
        }

        [EnterCommand(
            Command = "权限验证开启",
            Description = "开启权限验证",
            Syntax = "",
            Tag = "系统命令",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool AuthEnable(MsgInformationEx MsgDTO, object[] param)
        {
            const string key = "AuthDisable";
            SCacheService.Cache(key, "Disable", DateTime.Now);

            MsgSender.Instance.PushMsg(MsgDTO, "验证已开启！");
            return true;
        }

        [EnterCommand(
            Command = "功能奖励",
            Description = "奖励某个人某个功能若个使用次数（当日有效）",
            Syntax = "[命令名] [@QQ号] [奖励个数]",
            Tag = "系统命令",
            SyntaxChecker = "Word At Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = false)]
        public bool FishingBonus(MsgInformationEx MsgDTO, object[] param)
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
            return true;
        }

        [EnterCommand(Command = "物品奖励",
            Description = "奖励某个人某个物品",
            Syntax = "[@QQ号] [物品名]",
            Tag = "系统命令",
            SyntaxChecker = "At Word",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = false)]
        public bool ItemBonus(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long) param[0];
            var itemName = param[1] as string;

            var item = HonorHelper.Instance.FindItem(itemName);
            if (item == null)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "未找到该物品！");
                return false;
            }

            var (msg, _) = ItemHelper.Instance.ItemIncome(qqNum, itemName);
            if (!string.IsNullOrEmpty(msg))
            {
                MsgSender.Instance.PushMsg(MsgDTO, msg);
            }

            MsgSender.Instance.PushMsg(MsgDTO, "奖励已生效！");
            return true;
        }

        [EnterCommand(
            Command = "BlackList",
            Description = "Put someone to blacklist",
            Syntax = "qqnum",
            Tag = "系统命令",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool BlackList(MsgInformationEx MsgDTO, object[] param)
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
            return true;
        }

        [EnterCommand(
            Command = "FreeBlackList",
            Description = "Pull someone out from blacklist",
            Syntax = "qqnum",
            Tag = "系统命令",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool FreeBlackList(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long) param[0];
            var query = MongoService<BlackList>.Get(b => b.QQNum == qqNum).FirstOrDefault();
            if (query == null)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "Not In BlackList");
                return false;
            }

            MongoService<BlackList>.Delete(query);
            MsgSender.Instance.PushMsg(MsgDTO, "Success");
            return true;
        }

        [EnterCommand(
            Command = "驱散",
            Description = "清除某人身上的所有buff",
            Syntax = "[@QQ]",
            Tag = "系统命令",
            SyntaxChecker = "At",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = false)]
        public bool Dispel(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long) param[0];
            var osPerson = OSPerson.GetPerson(qqNum);
            osPerson.Buffs.Clear();
            MongoService<OSPerson>.Update(osPerson);

            MsgSender.Instance.PushMsg(MsgDTO, "驱散成功！");
            return true;
        }

        [EnterCommand(
            Command = "驱散",
            Description = "清除某人身上的指定buff",
            Syntax = "[@QQ] [Buff名称]",
            Tag = "系统命令",
            SyntaxChecker = "At Word",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = false)]
        public bool DispelOneBuff(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long) param[0];
            var buffName = param[1] as string;

            var osPerson = OSPerson.GetPerson(qqNum);
            if (!osPerson.CheckBuff(buffName))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "目标身上没有指定buff！");
                return false;
            }

            osPerson.RemoveBuff(buffName);
            MongoService<OSPerson>.Update(osPerson);

            MsgSender.Instance.PushMsg(MsgDTO, "驱散成功！");
            return true;
        }
    }
}
