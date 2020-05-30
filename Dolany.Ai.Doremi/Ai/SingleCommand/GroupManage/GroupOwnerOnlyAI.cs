using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database.Ai;
using Dolany.WorldLine.Doremi.OnlineStore;

namespace Dolany.WorldLine.Doremi.Ai.SingleCommand.GroupManage
{
    public class GroupOwnerOnlyAI : AIBase
    {
        public override string AIName { get; set; } = "群主特权";
        public override string Description { get; set; } = "AI for some power only for group owners.";
        protected override CmdTagEnum DefaultTag { get; } = CmdTagEnum.群管理;

        public CrossWorldAiSvc CrossWorldAiSvc { get; set; }

        [EnterCommand(ID = "GroupOwnerOnlyAI_RefreshCommand",
            Command = "刷新",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "刷新某人某个功能的CD",
            SyntaxHint = "[@qq号] [命令名]",
            SyntaxChecker = "At Word",
            DailyLimit = 1)]
        public bool RefreshCommand(MsgInformationEx MsgDTO, object[] param)
        {
            var aimQQ = (long) param[0];
            var command = param[1] as string;
            var enter = CrossWorldAiSvc[MsgDTO.FromGroup].AllAvailableGroupCommands.FirstOrDefault(p => p.CommandsList.Contains(command));
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
            SyntaxHint = "[@QQ]",
            SyntaxChecker = "At",
            AuthorityLevel = AuthorityLevel.群主)]
        public bool Dispel(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long) param[0];

            var sourcePerson = OSPerson_Doremi.GetPerson(MsgDTO.FromQQ);
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
            SyntaxHint = "[@QQ] [Buff名称]",
            SyntaxChecker = "At Word",
            AuthorityLevel = AuthorityLevel.群主)]
        public bool DispelOneBuff(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long) param[0];
            var buffName = param[1] as string;

            if (!OSPersonBuff.CheckBuff(qqNum, buffName))
            {
                MsgSender.PushMsg(MsgDTO, "目标身上没有指定buff！");
                return false;
            }

            var sourcePerson = OSPerson_Doremi.GetPerson(MsgDTO.FromQQ);
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

        [EnterCommand(ID = "GroupOwnerOnlyAI_ExchangeOwner",
            Command = "移交群主",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "将群主身份移交给指定群员",
            SyntaxHint = "[@QQ号]",
            SyntaxChecker = "At")]
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

            MsgSender.PushMsg(MsgDTO, "已成功移交群主！");

            return true;
        }
    }
}
