using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Ai.Game.Gift;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database;
using Dolany.Database.Ai;
using Dolany.Database.Sqlite;
using Newtonsoft.Json;

namespace Dolany.Ai.Core.Ai.Sys
{
    [AI(Name = "开发者后台",
        Enable = true,
        Description = "Ai for developer only operations",
        PriorityLevel = 10)]
    public class DeveloperOnlyAI : AIBase
    {
        [EnterCommand(ID = "DeveloperOnlyAI_Board",
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
            var groups = GroupSettingMgr.Instance.SettingDic.Values.Where(g => g.ExpiryTime.HasValue && g.ExpiryTime.Value > DateTime.Now);

            foreach (var group in groups)
            {
                MsgSender.PushMsg(
                    new MsgCommand { Command = CommandType.SendGroup, Msg = content, ToGroup = group.GroupNum, BindAi = group.BindAi});

                Thread.Sleep(2000);
            }

            MsgSender.PushMsg(MsgDTO, "广播结束！");
            return true;
        }

        [EnterCommand(ID = "DeveloperOnlyAI_FishingBonus",
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
            var count = (int) (long) param[2];

            var enter = AIMgr.Instance.AllAvailableGroupCommands.FirstOrDefault(p => p.CommandsList.Contains(command));
            if (enter == null)
            {
                MsgSender.PushMsg(MsgDTO, "未找到该功能！", true);
                return false;
            }

            var dailyLimit = DailyLimitRecord.Get(qqNum, enter.ID);
            dailyLimit.Decache(count);
            dailyLimit.Update();

            MsgSender.PushMsg(MsgDTO, "奖励已生效！");
            return true;
        }

        [EnterCommand(ID = "DeveloperOnlyAI_ItemBonus",
            Command = "物品奖励",
            Description = "奖励某个人若干个物品",
            Syntax = "[@QQ号] [物品名] [物品数量]",
            Tag = "系统命令",
            SyntaxChecker = "At Word Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = false)]
        public bool ItemBonus(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long) param[0];
            var itemName = param[1] as string;
            var count = (int) (long) param[2];

            var item = HonorHelper.Instance.FindItem(itemName);
            if (item == null)
            {
                MsgSender.PushMsg(MsgDTO, "未找到该物品！");
                return false;
            }

            var record = ItemCollectionRecord.Get(qqNum);
            var msg = record.ItemIncome(itemName, count);
            if (!string.IsNullOrEmpty(msg))
            {
                MsgSender.PushMsg(MsgDTO, msg);
            }

            MsgSender.PushMsg(MsgDTO, "奖励已生效！");
            return true;
        }

        [EnterCommand(ID = "DeveloperOnlyAI_GoldBonus",
            Command = "金币奖励",
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

        [EnterCommand(ID = "DeveloperOnlyAI_GiftBonus",
            Command = "礼物奖励",
            Description = "奖励某个人若干件礼物",
            Syntax = "[@QQ号] [礼物名称] [礼物数量]",
            Tag = "系统命令",
            SyntaxChecker = "At Word Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = false)]
        public bool GiftBonus(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long) param[0];
            var name = param[1] as string;
            var count = (int) (long) param[2];

            var gift = GiftMgr.Instance[name];
            if(gift == null)
            {
                MsgSender.PushMsg(MsgDTO, "未找到该礼物！");
                return false;
            }

            var osPerson = OSPerson.GetPerson(qqNum);
            osPerson.GiftIncome(name, count);
            osPerson.Update();

            MsgSender.PushMsg(MsgDTO, "奖励已生效！");
            return true;
        }

        [EnterCommand(ID = "DeveloperOnlyAI_BlackList",
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

            DirtyFilter.Instance.Refresh();

            MsgSender.PushMsg(MsgDTO, "Success");
            return true;
        }

        [EnterCommand(ID = "DeveloperOnlyAI_FreeBlackList",
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

            DirtyFilter.Instance.Refresh();
            MsgSender.PushMsg(MsgDTO, "Success");
            return true;
        }

        [EnterCommand(ID = "DeveloperOnlyAI_InitAi",
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
            if (!GroupMemberInfoCacher.RefreshGroupInfo(groupNum, MsgDTO.BindAi))
            {
                MsgSender.PushMsg(MsgDTO, "初始化失败，请稍后再试！");
                return false;
            }

            MsgSender.PushMsg(MsgDTO, "初始化成功！");
            return true;
        }

        [EnterCommand(ID = "DeveloperOnlyAI_Register",
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
                BindAi = MsgDTO.BindAi
            };
            MongoService<GroupSettings>.Insert(setting);
            GroupSettingMgr.Instance.Refresh();

            MsgSender.PushMsg(MsgDTO, "注册成功！");
            return true;
        }

        [EnterCommand(ID = "DeveloperOnlyAI_BonusChance",
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
            var personCache = PersonCacheRecord.Get(aimNum, "抽奖");
            personCache.Value = int.TryParse(personCache.Value, out var times) ? (times + 1).ToString() : 1.ToString();
            personCache.Update();

            MsgSender.PushMsg(MsgDTO, "奖励已生效！");

            return true;
        }

        [EnterCommand(ID = "DeveloperOnlyAI_Freeze",
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

        [EnterCommand(ID = "DeveloperOnlyAI_Defreeze",
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

        [EnterCommand(ID = "DeveloperOnlyAI_ChargeTime",
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
            if (setting.ExpiryTime == null || setting.ExpiryTime.Value < DateTime.Now)
            {
                setting.ExpiryTime = DateTime.Now.AddDays(days);
            }
            else
            {
                setting.ExpiryTime = setting.ExpiryTime.Value.AddDays(days);
            }
            setting.Update();

            GroupSettingMgr.Instance.Refresh();

            MsgSender.PushMsg(MsgDTO, "充值成功");
            return true;
        }

        [EnterCommand(ID = "DeveloperOnlyAI_BindAi",
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
            setting.BindAi = MsgDTO.BindAi;
            setting.Update();

            GroupSettingMgr.Instance.Refresh();

            MsgSender.PushMsg(MsgDTO, "绑定成功");
            return true;
        }

        [EnterCommand(ID = "DeveloperOnlyAI_SignInAcc",
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

            var record = GlobalVarRecord.Get("SignInAcc");
            record.Value = "any";
            record.ExpiryTime = DateTime.Now.AddDays(days);
            record.Update();

            MsgSender.PushMsg(MsgDTO, "开启成功");
            return true;
        }

        [EnterCommand(ID = "DeveloperOnlyAI_GetCache",
            Command = "查询缓存 查看缓存",
            Description = "根据key值查询缓存信息",
            Syntax = "[key]",
            Tag = "系统命令",
            SyntaxChecker = "Word",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool GetCache(MsgInformationEx MsgDTO, object[] param)
        {
            var key = param[0] as string;
            using (var cache = new SqliteContext(Configger<AIConfigBase>.Instance.AIConfig.CacheDb))
            {
                var content = cache.SqliteCacheModel.FirstOrDefault(p => p.Key == key);
                if (content == null)
                {
                    MsgSender.PushMsg(MsgDTO, "nothing");
                    return false;
                }

                var json = JsonConvert.SerializeObject(content);
                MsgSender.PushMsg(MsgDTO, json);
            }

            return true;
        }

        [EnterCommand(ID = "DeveloperOnlyAI_CleanCache",
            Command = "清理缓存 删除缓存",
            Description = "根据key值删除缓存信息",
            Syntax = "[key]",
            Tag = "系统命令",
            SyntaxChecker = "Word",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool CleanCache(MsgInformationEx MsgDTO, object[] param)
        {
            var key = param[0] as string;
            using (var cache = new SqliteContext(Configger<AIConfigBase>.Instance.AIConfig.CacheDb))
            {
                var content = cache.SqliteCacheModel.FirstOrDefault(p => p.Key == key);
                if (content == null)
                {
                    MsgSender.PushMsg(MsgDTO, "nothing");
                    return false;
                }

                content.ExpTime = DateTime.Now.ToString(CultureInfo.CurrentCulture);
                cache.SaveChanges();
                MsgSender.PushMsg(MsgDTO, "completed");
            }

            return true;
        }
    }
}
