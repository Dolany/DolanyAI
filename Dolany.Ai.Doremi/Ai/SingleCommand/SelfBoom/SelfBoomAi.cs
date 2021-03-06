﻿using System;
using System.Threading;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.UtilityTool;

namespace Dolany.WorldLine.Doremi.Ai.SingleCommand.SelfBoom
{
    public class SelfBoomAi : AIBase
    {
        public override string AIName { get; set; } = "自爆";
        public override string Description { get; set; } = "AI for boom herself.";
        protected override CmdTagEnum DefaultTag { get; } = CmdTagEnum.系统命令;

        private int BoomCode = Rander.RandInt(100000);
        private DateTime CodeDate = DateTime.Now;

        [EnterCommand(ID = "SelfBoomAi_Boom",
            Command = "Boom",
            Description = "自爆")]
        public bool Boom(MsgInformationEx MsgDTO, object[] param)
        {
            var backInfo = WaiterSvc.WaitForInformation(
                new MsgCommand
                    {
                        Command = CommandType.SendGroup,
                        Msg = "请于5秒内输入指令码！",
                        ToGroup = MsgDTO.FromGroup,
                        BindAi = MsgDTO.BindAi
                    },
                info => info.Msg == BoomCode.ToString());
            if (backInfo == null)
            {
                MsgSender.PushMsg(MsgDTO, "自爆失败！");
                return false;
            }

            if (CodeDate.AddMinutes(5) < DateTime.Now)
            {
                MsgSender.PushMsg(MsgDTO, "指令码已失效！");
                return false;
            }

            MsgSender.PushMsg(MsgDTO, "AI即将自爆！");
            Thread.Sleep(1000);

            for (var i = 5; i > 0; i--)
            {
                MsgSender.PushMsg(MsgDTO, i.ToString());
                Thread.Sleep(1000);
            }

            MsgSender.PushMsg(MsgDTO, CodeApi.Code_Image_Relational("images/boom.jpg"));

            BoomCode = Rander.RandInt(10000);
            Thread.Sleep(1000);

            return true;
        }

        [EnterCommand(ID = "SelfBoomAi_GetBoomCode",
            Command = "BoomCode",
            AuthorityLevel = AuthorityLevel.开发者,
            Description = "获取自爆指令码，有效期5分钟",
            IsPrivateAvailable = true)]
        public bool GetBoomCode(MsgInformationEx MsgDTO, object[] param)
        {
            BoomCode = Rander.RandInt(100000);
            CodeDate = DateTime.Now;
            MsgSender.PushMsg(MsgDTO, BoomCode.ToString());
            return true;
        }
    }
}
