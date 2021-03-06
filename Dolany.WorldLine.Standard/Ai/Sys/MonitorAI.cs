﻿using System;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.WorldLine.Standard.OnlineStore;

namespace Dolany.WorldLine.Standard.Ai.Sys
{
    public class MonitorAI : AIBase
    {
        public override string AIName { get; set; } = "监视器";

        public override string Description { get; set; } = "AI for Monitoring and managing Ais status.";

        public override AIPriority PriorityLevel { get;} = AIPriority.Monitor;

        protected override CmdTagEnum DefaultTag { get; } = CmdTagEnum.系统命令;

        [EnterCommand(ID = "MonitorAI_SelfExcharge",
            Command = "自助充值",
            Description = "使用钻石自助为本群充值指定天数的机器人使用时间(1天=10钻石)",
            SyntaxHint = "[天数]",
            SyntaxChecker = "Long")]
        public bool SelfExcharge(MsgInformationEx MsgDTO, object[] param)
        {
            var days = (int) (long) param[0];
            if (days <= 0)
            {
                MsgSender.PushMsg(MsgDTO, "天数错误，请重新输入命令！");
                return false;
            }

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            var diamondNeed = days * 10;
            if (osPerson.Diamonds < diamondNeed)
            {
                MsgSender.PushMsg(MsgDTO, "你的钻石余额不足，请添加能天使(2731544408)为好友后，使用【转账】功能转任意金额后将会获得金额*100的钻石，可以【我的状态】命令查看余额！");
                return false;
            }

            ChargeTime(MsgDTO, new object[] {MsgDTO.FromGroup, (long)days});
            osPerson.Diamonds -= diamondNeed;
            osPerson.Update();

            return true;
        }

        public void ChargeTime(MsgInformationEx MsgDTO, object[] param)
        {
            var groupNum = (long) param[0];
            var days = (int) (long) param[1];

            var setting = GroupSettingSvc[groupNum];
            if (setting.ExpiryTime == null || setting.ExpiryTime.Value < DateTime.Now)
            {
                setting.ExpiryTime = DateTime.Now.AddDays(days);
            }
            else
            {
                setting.ExpiryTime = setting.ExpiryTime.Value.AddDays(days);
            }
            setting.Update();

            MsgSender.PushMsg(MsgDTO, $"充值成功，有效期至 {setting.ExpiryTime:yyyy-MM-dd HH:mm:ss}");
        }
    }
}
