using System.Linq;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.Model;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Ai.SingleCommand.GroupOwnerOnly
{
    [AI(
        Name = "群主特权",
        Description = "AI for some power only for group owners.",
        Enable = true,
        PriorityLevel = 10)]
    public class GroupOwnerOnlyAI : AIBase
    {
        [EnterCommand(ID = "GroupOwnerOnlyAI_RefreshCommand",
            Command = "刷新",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "刷新某人某个功能的CD",
            Syntax = "[@qq号] [命令名]",
            Tag = "群主特权",
            SyntaxChecker = "At Word",
            IsPrivateAvailable = false,
            DailyLimit = 1)]
        public bool RefreshCommand(MsgInformationEx MsgDTO, object[] param)
        {
            var aimQQ = (long) param[0];
            var command = param[1] as string;
            var enter = AIMgr.Instance.AllAvailableGroupCommands.FirstOrDefault(p => p.CommandsList.Contains(command));
            if (enter == null)
            {
                MsgSender.PushMsg(MsgDTO, "未找到该功能！", true);
                return false;
            }

            var dailyLimit = DailyLimitRecord.Get(aimQQ, enter.ID);
            dailyLimit.Times = 0;
            dailyLimit.Update();

            MsgSender.PushMsg(MsgDTO, "刷新成功！");

            return true;
        }

        [EnterCommand(ID = "GroupOwnerOnlyAI_Dispel",
            Command = "驱散",
            Description = "清除某人身上的所有buff",
            Syntax = "[@QQ]",
            Tag = "群主特权",
            SyntaxChecker = "At",
            AuthorityLevel = AuthorityLevel.群主,
            IsPrivateAvailable = false)]
        public bool Dispel(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long) param[0];

            var sourcePerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (sourcePerson.Golds < 500)
            {
                MsgSender.PushMsg(MsgDTO, "驱散全部buff需要500金币，你没有足够的金币！");
                return false;
            }

            if (!Waiter.Instance.WaitForConfirm_Gold(MsgDTO, 500))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            OSPersonBuff.RemoveAll(qqNum);

            sourcePerson.Golds -= 500;
            sourcePerson.Update();

            MsgSender.PushMsg(MsgDTO, "驱散成功！");
            return true;
        }

        [EnterCommand(ID = "GroupOwnerOnlyAI_DispelOneBuff",
            Command = "驱散",
            Description = "清除某人身上的指定buff",
            Syntax = "[@QQ] [Buff名称]",
            Tag = "群主特权",
            SyntaxChecker = "At Word",
            AuthorityLevel = AuthorityLevel.群主,
            IsPrivateAvailable = false)]
        public bool DispelOneBuff(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long) param[0];
            var buffName = param[1] as string;

            if (!OSPersonBuff.CheckBuff(qqNum, buffName))
            {
                MsgSender.PushMsg(MsgDTO, "目标身上没有指定buff！");
                return false;
            }

            var sourcePerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (sourcePerson.Golds < 100)
            {
                MsgSender.PushMsg(MsgDTO, "驱散该buff需要100金币，你没有足够的金币！");
                return false;
            }

            if (!Waiter.Instance.WaitForConfirm_Gold(MsgDTO, 100))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            OSPersonBuff.Remove(qqNum, buffName);

            sourcePerson.Golds -= 100;
            sourcePerson.Update();

            MsgSender.PushMsg(MsgDTO, "驱散成功！");
            return true;
        }

        [EnterCommand(ID = "GroupOwnerOnlyAI_EnableAllModules",
            Command = "开启所有功能",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "开启机器人的所有功能",
            Syntax = "",
            Tag = "群主特权",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool EnableAllModules(MsgInformationEx MsgDTO, object[] param)
        {
            var setting = GroupSettingMgr.Instance[MsgDTO.FromGroup];
            setting.EnabledFunctions = AIMgr.Instance.OptionalAINames;
            setting.Update();

            MsgSender.PushMsg(MsgDTO, "开启成功！");
            return true;
        }

        [EnterCommand(ID = "GroupOwnerOnlyAI_ViewAllOptionalModules",
            Command = "可选功能列表",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "查看机器人的所有可选功能",
            Syntax = "",
            Tag = "群主特权",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool ViewAllOptionalModules(MsgInformationEx MsgDTO, object[] param)
        {
            var setting = GroupSettingMgr.Instance[MsgDTO.FromGroup];
            var allModules = AIMgr.Instance.OptionalAINames;

            var msgs = allModules.Select(m => $"{m}  {(setting.EnabledFunctions.Contains(m) ? "√" : "×")}");
            var msg = $"{string.Join("\r", msgs)}\r可以使用 开启功能 [功能名] 来开启对应的功能；或使用 关闭功能 [功能名] 来关闭对应的功能";
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }
    }
}
