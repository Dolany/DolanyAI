using System.Threading;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Sys
{
    using System;
    using System.Linq;
    using Base;
    using Cache;
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
                MsgSender.PushMsg(MsgDTO, "权限名称错误！");
                return false;
            }

            var key = $"TempAuthorize-{MsgDTO.FromGroup}-{qqNum}";
            var model = new TempAuthorizeCache { AuthName = authName, GroupNum = MsgDTO.FromGroup, QQNum = qqNum };
            SCacheService.Cache(key, model);

            MsgSender.PushMsg(MsgDTO, "临时授权成功！");
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
            var groups = AIMgr.Instance.AllGroupsDic.Keys;

            foreach (var group in groups)
            {
                MsgSender.PushMsg(
                    new MsgCommand { Command = AiCommand.SendGroup, Msg = content, ToGroup = group });

                Thread.Sleep(2000);
            }

            MsgSender.PushMsg(MsgDTO, "广播结束！");
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

            MsgSender.PushMsg(MsgDTO, "奖励已生效！");
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
                MsgSender.PushMsg(MsgDTO, "未找到该物品！");
                return false;
            }

            var record = DriftItemRecord.GetRecord(qqNum);
            var msg = record.ItemIncome(itemName);
            if (!string.IsNullOrEmpty(msg))
            {
                MsgSender.PushMsg(MsgDTO, msg);
            }

            MsgSender.PushMsg(MsgDTO, "奖励已生效！");
            return true;
        }

        [EnterCommand(Command = "金币奖励",
            Description = "奖励某个人一些金币",
            Syntax = "[@QQ号] [金币数量]",
            Tag = "系统命令",
            SyntaxChecker = "At Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = false)]
        public bool GoldBonus(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long) param[0];
            var golds = (int)(long)param[1];

            var osPerson = OSPerson.GetPerson(qqNum);
            osPerson.Golds += golds;
            osPerson.Update();

            MsgSender.PushMsg(MsgDTO, "奖励已生效！");
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
            var query = MongoService<BlackList>.GetOnly(b => b.QQNum == qqNum);
            if (query == null)
            {
                MongoService<BlackList>.Insert(new BlackList{QQNum = qqNum, BlackCount = 10, UpdateTime = DateTime.Now});
            }
            else
            {
                query.BlackCount = 10;
                MongoService<BlackList>.Update(query);
            }

            MsgSender.PushMsg(MsgDTO, "Success");
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
            var query = MongoService<BlackList>.GetOnly(b => b.QQNum == qqNum);
            if (query == null)
            {
                MsgSender.PushMsg(MsgDTO, "Not In BlackList");
                return false;
            }

            MongoService<BlackList>.Delete(query);
            MsgSender.PushMsg(MsgDTO, "Success");
            return true;
        }

        [EnterCommand(
            Command = "初始化",
            Description = "初始化群成员信息",
            Syntax = "[群号]",
            Tag = "系统命令",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool InitAi(MsgInformationEx MsgDTO, object[] param)
        {
            var groupNum = (long) param[0];
            if (!GroupMemberInfoCacher.RefreshGroupInfo(groupNum))
            {
                MsgSender.PushMsg(MsgDTO, "初始化失败，请稍后再试！");
                return false;
            }

            MsgSender.PushMsg(MsgDTO, "初始化成功！");
            return true;
        }

        [EnterCommand(
            Command = "注册",
            Description = "注册新的群组",
            Syntax = "[群号] [群名]",
            Tag = "系统命令",
            SyntaxChecker = "Long Word",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool Register(MsgInformationEx MsgDTO, object[] param)
        {
            var groupNum = (long) param[0];
            var name = param[1] as string;

            MongoService<GroupSettings>.DeleteMany(r => r.GroupNum == groupNum);
            var setting = new GroupSettings()
            {
                GroupNum = groupNum,
                Name = name,
                BindAi = Configger.Instance["BindAi"]
            };
            MongoService<GroupSettings>.Insert(setting);
            GroupSettingMgr.Instance.Refresh();

            MsgSender.PushMsg(MsgDTO, "注册成功！");
            return true;
        }

        [EnterCommand(
            Command = "抽奖奖励",
            Description = "奖励某个人一次抽奖机会",
            Syntax = "[QQ号]",
            Tag = "系统命令",
            SyntaxChecker = "At",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = false)]
        public bool BonusChance(MsgInformationEx MsgDTO, object[] param)
        {
            var aimNum = (long) param[0];
            var key = $"LimitBonus-{aimNum}";
            SCacheService.Cache(key, "nothing");

            MsgSender.PushMsg(MsgDTO, "奖励已生效！");

            return true;
        }

        [EnterCommand(
            Command = "冻结",
            Description = "冻结某个群的机器人",
            Syntax = "[群组号]",
            Tag = "系统命令",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool Freeze(MsgInformationEx MsgDTO, object[] param)
        {
            var groupNum = (long) param[0];
            if (!GroupSettingMgr.Instance.SettingDic.ContainsKey(groupNum))
            {
                MsgSender.PushMsg(MsgDTO, "未找到相关群组");
                return false;
            }

            var setting = GroupSettingMgr.Instance[groupNum];
            setting.ForcedShutDown = true;
            setting.Update();

            MsgSender.PushMsg(MsgDTO, "冻结成功");

            return true;
        }

        [EnterCommand(
            Command = "解冻",
            Description = "解冻某个群的机器人",
            Syntax = "[群组号]",
            Tag = "系统命令",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool Defreeze(MsgInformationEx MsgDTO, object[] param)
        {
            var groupNum = (long) param[0];
            if (!GroupSettingMgr.Instance.SettingDic.ContainsKey(groupNum))
            {
                MsgSender.PushMsg(MsgDTO, "未找到相关群组");
                return false;
            }

            var setting = GroupSettingMgr.Instance[groupNum];
            setting.ForcedShutDown = false;
            setting.Update();

            MsgSender.PushMsg(MsgDTO, "解冻成功");

            return true;
        }

        [EnterCommand(
            Command = "充值时间",
            Description = "给某个群组充值时间(单位天)",
            Syntax = "[群组号] [天数]",
            Tag = "系统命令",
            SyntaxChecker = "Long Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool ChargeTime(MsgInformationEx MsgDTO, object[] param)
        {
            var groupNum = (long) param[0];
            var days = (int) (long) param[1];

            var setting = MongoService<GroupSettings>.GetOnly(p => p.GroupNum == groupNum);
            setting.ExpiryTime = setting.ExpiryTime?.AddDays(days) ?? DateTime.Now.AddDays(days);
            setting.Update();

            GroupSettingMgr.Instance.Refresh();

            MsgSender.PushMsg(MsgDTO, "充值成功");

            return true;
        }

        [EnterCommand(
            Command = "绑定",
            Description = "将机器人绑定某个群组",
            Syntax = "[群组号]",
            Tag = "系统命令",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool BindAi(MsgInformationEx MsgDTO, object[] param)
        {
            var groupNum = (long) param[0];

            var setting = MongoService<GroupSettings>.GetOnly(p => p.GroupNum == groupNum);
            setting.BindAi = Configger.Instance["BindAi"];
            setting.Update();

            GroupSettingMgr.Instance.Refresh();

            MsgSender.PushMsg(MsgDTO, "绑定成功");

            return true;
        }

        [EnterCommand(
            Command = "签到加速",
            Description = "开启签到加速活动",
            Syntax = "[天数]",
            Tag = "系统命令",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool SignInAcc(MsgInformationEx MsgDTO, object[] param)
        {
            var days = (int) (long) param[0];

            var key = "SignInAcc";
            SCacheService.Cache(key, "SignInAcc", DateTime.Now.AddDays(days));

            MsgSender.PushMsg(MsgDTO, "开启成功");

            return true;
        }
    }
}
