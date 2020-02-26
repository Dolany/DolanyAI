using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.API;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database.Ai;
using Dolany.Database.Sqlite;
using Dolany.WorldLine.Standard.Ai.Game.Gift;
using Dolany.WorldLine.Standard.Ai.Game.Pet;
using Dolany.WorldLine.Standard.OnlineStore;
using Newtonsoft.Json;

namespace Dolany.WorldLine.Standard.Ai.Sys
{
    public class DeveloperOnlyAI : AIBase
    {
        public override string AIName { get; set; } = "开发者后台";

        public override string Description { get; set; } = "Ai for developer only operations.";

        public override AIPriority PriorityLevel { get;} = AIPriority.Monitor;

        private static GiftMgr GiftMgr => AutofacSvc.Resolve<GiftMgr>();
        private static HonorHelper HonorHelper => AutofacSvc.Resolve<HonorHelper>();

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
            var groups = GroupSettingMgr.SettingDic.Values.Where(g => g.ExpiryTime.HasValue && g.ExpiryTime.Value > DateTime.Now);

            foreach (var group in groups)
            {
                MsgSender.PushMsg(
                    new MsgCommand { Command = CommandType.SendGroup, Msg = content, ToGroup = group.GroupNum, BindAi = group.BindAi});

                Thread.Sleep(2000);
            }

            MsgSender.PushMsg(MsgDTO, "广播结束！");
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

            var item = HonorHelper.FindItem(itemName);
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

        [EnterCommand(ID = "DeveloperOnlyAI_DiamondBonus",
            Command = "钻石奖励",
            Description = "奖励某个人一些钻石",
            Syntax = "[@QQ号] [钻石数量]",
            Tag = "开发者后台",
            SyntaxChecker = "At Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = false)]
        public bool DiamondBonus(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long) param[0];
            var diamonds = (int)(long)param[1];

            var osPerson = OSPerson.GetPerson(qqNum);
            osPerson.Diamonds += diamonds;
            osPerson.Update();

            MsgSender.PushMsg(MsgDTO, "奖励已生效！");
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

            var gift = GiftMgr[name];
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
            var emojis = Emoji.AllEmojis();
            var msg = string.Join(" ", emojis);
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "DeveloperOnlyAI_ConnectiongState",
            Command = "连接状态",
            Description = "获取当前所有机器人的连接状态",
            Syntax = "",
            Tag = "开发者后台",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool ConnectiongState(MsgInformationEx MsgDTO, object[] param)
        {
            var command = new MsgCommand()
            {
                Command = CommandType.ConnectionState,
                BindAi = MsgDTO.BindAi
            };

            var info = Waiter.WaitForRelationId(command);
            if (info == null)
            {
                MsgSender.PushMsg(MsgDTO, "超时！");
                return false;
            }

            var dic = JsonConvert.DeserializeObject<Dictionary<string, bool>>(info.Msg);
            var msg = string.Join("\r", dic.Select(p => $"{p.Key}:{(p.Value ? "连接中" : "已断开")}"));

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "DeveloperOnlyAI_GetQQInfo",
            Command = "获取QQ信息",
            Description = "获取指定QQ的信息(测试)",
            Syntax = "[QQ号]",
            Tag = "开发者后台",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool GetQQInfo(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long) param[0];
            var info = APIEx.GetQQInfo(qqNum, MsgDTO.BindAi);

            MsgSender.PushMsg(MsgDTO, info != null ? JsonConvert.SerializeObject(info) : "获取失败！");
            return true;
        }

        [EnterCommand(ID = "DeveloperOnlyAI_WithdrawMessage",
            Command = "撤回消息测试",
            Description = "撤回消息测试",
            Syntax = "[消息ID]",
            Tag = "开发者后台",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool WithdrawMessage(MsgInformationEx MsgDTO, object[] param)
        {
            var msgid = (long)param[0];

            var withdrawCommond = new MsgCommand()
            {
                Msg = msgid.ToString(),
                Command = CommandType.WithdrawMessage,
                ToGroup = MsgDTO.FromGroup,
                BindAi = MsgDTO.BindAi
            };
            MsgSender.PushMsg(withdrawCommond);
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
                {
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
                }
                case "OSPerson":
                {
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
                }
                case "Pet":
                {
                    if (!long.TryParse(value, out var personNum))
                    {
                        msg = $"规则引擎解析错误：{personNum}";
                        break;
                    }

                    var pet = PetRecord.Get(personNum);
                    msg = JsonConvert.SerializeObject(pet);
                    break;
                }
                default:
                {
                    msg = "未匹配到指定规则，请先查阅世界规则手册！";
                    break;
                }
            }

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }
    }
}
