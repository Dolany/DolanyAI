﻿using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Ai.SingleCommand.SelfBoom
{
    using System.Threading;

    using API;
    using Base;
    using Cache;
    using Model;

    [AI(
        Name = "自爆",
        Description = "AI for boom herself.",
        Enable = true,
        PriorityLevel = 10)]
    public class SelfBoomAi : AIBase
    {
        private int BoomCode = CommonUtil.RandInt(10000);

        [EnterCommand(
            Command = "Boom",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "自爆",
            Syntax = "",
            Tag = "系统命令",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool Boom(MsgInformationEx MsgDTO, object[] param)
        {
            var backInfo = Waiter.Instance.WaitForInformation(
                new MsgCommand
                    {
                        Command = AiCommand.SendGroup,
                        Msg = "请于5秒内输入指令码！",
                        ToGroup = MsgDTO.FromGroup
                    },
                info => info.Msg == BoomCode.ToString());
            if (backInfo == null)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "自爆失败！");
                return false;
            }

            Thread.Sleep(1000);
            MsgSender.Instance.PushMsg(MsgDTO, "AI即将自爆！");
            for (var i = 5; i > 0; i--)
            {
                MsgSender.Instance.PushMsg(MsgDTO, i.ToString());
                Thread.Sleep(1000);
            }

            MsgSender.Instance.PushMsg(MsgDTO, CodeApi.Code_Image_Relational("images/boom.jpg"));

            BoomCode = CommonUtil.RandInt(10000);
            Thread.Sleep(1000);
            MsgSender.Instance.PushMsg(
                new MsgCommand
                    {
                        Command = AiCommand.Restart
                    });
            return true;
        }

        [EnterCommand(
            Command = "BoomCode",
            AuthorityLevel = AuthorityLevel.开发者,
            Description = "获取自爆指令码",
            Syntax = "",
            Tag = "系统功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true)]
        public bool GetBoomCode(MsgInformationEx MsgDTO, object[] param)
        {
            MsgSender.Instance.PushMsg(MsgDTO, BoomCode.ToString());
            return true;
        }

        public override void Initialization()
        {
        }
    }
}
