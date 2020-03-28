using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Doremi.Base;
using Dolany.Ai.Doremi.Cache;
using Dolany.Ai.Doremi.Common;
using Dolany.Ai.Doremi.OnlineStore;
using Dolany.Database.Ai;

namespace Dolany.Ai.Doremi.Ai.SingleCommand.GroupManage
{
    [AI(Name = "群主特权",
        Description = "AI for some power only for group owners.",
        Enable = true,
        PriorityLevel = 10,
        BindAi = "Doremi")]
    public class GroupOwnerOnlyAI : AIBase
    {
        private static WaiterSvc WaiterSvc => AutofacSvc.Resolve<WaiterSvc>();
        private static GroupSettingSvc GroupSettingSvc => AutofacSvc.Resolve<GroupSettingSvc>();

        [EnterCommand(ID = "GroupOwnerOnlyAI_RefreshCommand",
            Command = "刷新",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "刷新某人某个功能的CD",
            Syntax = "[@qq号] [命令名]",
            Tag = "群管理",
            SyntaxChecker = "At Word",
            IsPrivateAvailable = false,
            DailyLimit = 1)]
        public bool RefreshCommand(MsgInformationEx MsgDTO, object[] param)
        {
            var aimQQ = (long) param[0];
            var command = param[1] as string;
            var enter = AiSvc.AllAvailableGroupCommands.FirstOrDefault(p => p.CommandsList.Contains(command));
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
            Tag = "群管理",
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

            if (!WaiterSvc.WaitForConfirm_Gold(MsgDTO, 500))
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
            Tag = "群管理",
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

            if (!WaiterSvc.WaitForConfirm_Gold(MsgDTO, 100))
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
            Tag = "群管理",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool EnableAllModules(MsgInformationEx MsgDTO, object[] param)
        {
            var setting = GroupSettingSvc[MsgDTO.FromGroup];
            setting.EnabledFunctions = AiSvc.OptionalAINames;
            setting.Update();

            MsgSender.PushMsg(MsgDTO, "开启成功！");
            return true;
        }

        [EnterCommand(ID = "GroupOwnerOnlyAI_ViewAllOptionalModules",
            Command = "可选功能列表",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "查看机器人的所有可选功能",
            Syntax = "",
            Tag = "群管理",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool ViewAllOptionalModules(MsgInformationEx MsgDTO, object[] param)
        {
            var setting = GroupSettingSvc[MsgDTO.FromGroup];
            var allModules = AiSvc.OptionalAINames;

            var msgs = allModules.Select(m => $"{m}  {(setting.EnabledFunctions.Contains(m) ? "√" : "×")}");
            var msg = $"{string.Join("\r\n", msgs)}\r\n可以使用 开启功能 [功能名] 来开启对应的功能；或使用 关闭功能 [功能名] 来关闭对应的功能";
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "GroupOwnerOnlyAI_ExchangeOwner",
            Command = "移交群主",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "将群主身份移交给指定群员",
            Syntax = "[@QQ号]",
            Tag = "群管理",
            SyntaxChecker = "At",
            IsPrivateAvailable = false)]
        public bool ExchangeOwner(MsgInformationEx MsgDTO, object[] param)
        {
            var aimQQ = (long) param[0];
            if (!WaiterSvc.WaitForConfirm(MsgDTO, $"【警告】是否确认将群主移交给 {CodeApi.Code_At(aimQQ)}？（此操作不可逆）"))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            var setting = GroupSettingSvc[MsgDTO.FromGroup];
            setting.AuthInfo.Owner = aimQQ;

            setting.Update();
            GroupSettingSvc.Refresh();

            MsgSender.PushMsg(MsgDTO, "已成功移交群主！");

            return true;
        }
    }
}
