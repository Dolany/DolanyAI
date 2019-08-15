using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Ai.Game.Gift;
using Dolany.Ai.Core.Ai.Game.Pet;
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
    public class DeveloperOnlyAI : AIBase
    {
        public override string AIName { get; set; } = "开发者后台";

        public override string Description { get; set; } = "Ai for developer only operations.";

        public override int PriorityLevel { get; set; } = 10;

        [EnterCommand(ID = "DeveloperOnlyAI_Board",
            Command = "广播",
            Description = "在所有群组广播消息",
            Syntax = "广播内容",
            Tag = "开发者后台",
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
            Tag = "开发者后台",
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
            Tag = "开发者后台",
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
            Tag = "开发者后台",
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

        [EnterCommand(ID = "DeveloperOnlyAI_ForbiddenPicCache",
            Command = "禁用图片缓存",
            Description = "禁用一个群的图片缓存",
            Syntax = "[群号]",
            Tag = "开发者后台",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool ForbiddenPicCache(MsgInformationEx MsgDTO, object[] param)
        {
            var groupNum = (long) param[0];
            var groupSetting = GroupSettingMgr.Instance[groupNum];
            if (groupSetting.AdditionSettings == null)
            {
                groupSetting.AdditionSettings = new Dictionary<string, string>();
            }
            groupSetting.AdditionSettings.AddSafe("禁止图片缓存", true.ToString());
            groupSetting.Update();

            MsgSender.PushMsg(MsgDTO, "命令已完成！");
            return true;
        }

        [EnterCommand(ID = "DeveloperOnlyAI_GiftBonus",
            Command = "礼物奖励",
            Description = "奖励某个人若干件礼物",
            Syntax = "[@QQ号] [礼物名称] [礼物数量]",
            Tag = "开发者后台",
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

        [EnterCommand(ID = "DeveloperOnlyAI_PetExpBonus",
            Command = "宠物经验值奖励",
            Description = "奖励某个人若干宠物经验值",
            Syntax = "[@QQ号] [经验值]",
            Tag = "开发者后台",
            SyntaxChecker = "At Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = false)]
        public bool PetExpBonus(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long) param[0];
            var exp = (int) (long) param[1];

            var pet = PetRecord.Get(qqNum);
            MsgDTO.FromQQ = qqNum;
            var msg = pet.ExtGain(MsgDTO, exp);
            msg += "\r奖励已生效！";

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "DeveloperOnlyAI_BlackList",
            Command = "BlackList",
            Description = "Put someone to blacklist",
            Syntax = "qqnum",
            Tag = "开发者后台",
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
            Tag = "开发者后台",
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
            Tag = "开发者后台",
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
            Tag = "开发者后台",
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
                BindAi = MsgDTO.BindAi,
                BindAis = new List<string>(){MsgDTO.BindAi}
            };
            MongoService<GroupSettings>.Insert(setting);
            GroupSettingMgr.Instance.Refresh();

            MsgSender.PushMsg(MsgDTO, "注册成功！");
            return true;
        }

        [EnterCommand(ID = "DeveloperOnlyAI_BindAi",
            Command = "绑定",
            Description = "绑定机器人",
            Syntax = "[群号] [机器人名]",
            Tag = "开发者后台",
            SyntaxChecker = "Long Word",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool BindAi(MsgInformationEx MsgDTO, object[] param)
        {
            var groupNum = (long) param[0];
            var name = param[1] as string;

            if (!GroupSettingMgr.Instance.SettingDic.ContainsKey(groupNum))
            {
                MsgSender.PushMsg(MsgDTO, "错误的群号");
                return false;
            }

            if (!BindAiMgr.Instance.AiDic.ContainsKey(name))
            {
                MsgSender.PushMsg(MsgDTO, "错误的机器人名");
                return false;
            }

            var setting = GroupSettingMgr.Instance[groupNum];
            if (setting.BindAis == null)
            {
                setting.BindAis = new List<string>();
            }

            if (!setting.BindAis.Contains(name))
            {
                setting.BindAis.Add(name);
            }

            setting.Update();

            MsgSender.PushMsg(MsgDTO, "绑定成功！");
            return true;
        }

        [EnterCommand(ID = "DeveloperOnlyAI_BonusChance",
            Command = "抽奖奖励",
            Description = "奖励某个人一次抽奖机会",
            Syntax = "[QQ号]",
            Tag = "开发者后台",
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
            Tag = "开发者后台",
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
            Tag = "开发者后台",
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
            Tag = "开发者后台",
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

        [EnterCommand(ID = "DeveloperOnlyAI_SignInAcc",
            Command = "签到加速",
            Description = "开启签到加速活动",
            Syntax = "[天数]",
            Tag = "开发者后台",
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
            Tag = "开发者后台",
            SyntaxChecker = "Word",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool GetCache(MsgInformationEx MsgDTO, object[] param)
        {
            var key = param[0] as string;
            using (var cache = new SqliteContext(Global.DefaultConfig.CacheDb))
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
            Tag = "开发者后台",
            SyntaxChecker = "Word",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool CleanCache(MsgInformationEx MsgDTO, object[] param)
        {
            var key = param[0] as string;
            using (var cache = new SqliteContext(Global.DefaultConfig.CacheDb))
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

        [EnterCommand(ID = "DeveloperOnlyAI_Test",
            Command = "test",
            Description = "test",
            Syntax = "",
            Tag = "开发者后台",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool Test(MsgInformationEx MsgDTO, object[] param)
        {
            var info = Waiter.Instance.WaitForInformation(MsgDTO, "wait for pic",
                information => information.FromGroup == MsgDTO.FromGroup && information.FromQQ == MsgDTO.FromQQ &&
                               !string.IsNullOrEmpty(Utility.ParsePicGuid(information.Msg)), 10);
            if(info == null)
            {
                MsgSender.PushMsg(MsgDTO, "operation cancel!");
                return false;
            }

            var bindai = BindAiMgr.Instance[MsgDTO.BindAi];

            var picGuid = Utility.ParsePicGuid(info.Msg);
            var imageCache = Utility.ReadImageCacheInfo(picGuid, bindai.ImagePath);
            MsgSender.PushMsg(MsgDTO, imageCache?.url);
            var path = $"./images/Custom/Pet/{MsgDTO.FromQQ}.{imageCache?.type}";
            Utility.DownloadImage(imageCache?.url, path);

            MsgSender.PushMsg(MsgDTO, CodeApi.Code_Image_Relational(path));
            return true;
        }

        [EnterCommand(ID = "DeveloperOnlyAI_RuleDestruction",
            Command = "规则析构",
            Description = "规则析构",
            Syntax = "[key] [value]",
            Tag = "开发者后台",
            SyntaxChecker = "Word Any",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool RuleDestruction(MsgInformationEx MsgDTO, object[] param)
        {
            var key = param[0] as string;
            var value = param[1] as string;

            string msg;
            MsgSender.PushMsg(MsgDTO, "世界树规则析构引擎正在启动，请稍候...");
            Thread.Sleep(2000);

            switch (key)
            {
                case "GroupSettings":
                    if (!long.TryParse(value, out var groupNum))
                    {
                        msg = $"规则引擎解析错误：{groupNum}";
                        break;
                    }

                    var record = GroupSettings.Get(groupNum);
                    if (record == null)
                    {
                        msg = $"未匹配到基元数据：{groupNum}";
                        break;
                    }

                    msg = JsonConvert.SerializeObject(record);
                    break;
                case "OSPerson":
                    if (!long.TryParse(value, out var personNum))
                    {
                        msg = $"规则引擎解析错误：{personNum}";
                        break;
                    }

                    var osPerson = OSPerson.GetPerson(personNum);
                    if (osPerson == null)
                    {
                        msg = $"未匹配到基元数据：{personNum}";
                        break;
                    }

                    msg = JsonConvert.SerializeObject(osPerson);
                    break;
                default:
                    msg = "未匹配到指定规则，请先查阅世界规则手册！";
                    break;
            }

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }
    }
}
