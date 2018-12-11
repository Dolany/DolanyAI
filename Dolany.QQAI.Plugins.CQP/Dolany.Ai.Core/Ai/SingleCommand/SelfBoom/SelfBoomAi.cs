namespace Dolany.Ai.Core.Ai.SingleCommand.SelfBoom
{
    using System;
    using System.Threading;

    using Dolany.Ai.Core.API;
    using Dolany.Ai.Core.Base;
    using Dolany.Ai.Core.Cache;
    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Db;

    [AI(
        Name = nameof(SelfBoomAi),
        Description = "AI for boom herself.",
        IsAvailable = true,
        PriorityLevel = 10)]
    public class SelfBoomAi : AIBase
    {
        [EnterCommand(
            Command = "Boom",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "自爆",
            Syntax = "",
            Tag = "系统功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailabe = false)]
        public void Boom(MsgInformationEx MsgDTO, object[] param)
        {
            var backInfo = Waiter.Instance.WaitForInformation(
                new MsgCommand
                    {
                        Command = AiCommand.SendGroup,
                        Id = Guid.NewGuid().ToString(),
                        Msg = "请于5秒内输入指令码！",
                        Time = DateTime.Now,
                        ToGroup = MsgDTO.FromGroup
                    },
                info => info.Msg == Global.AuthCode);
            if (backInfo == null || string.IsNullOrEmpty(backInfo.Msg))
            {
                MsgSender.Instance.PushMsg(
                    new MsgCommand
                        {
                            Command = AiCommand.SendGroup,
                            Id = Guid.NewGuid().ToString(),
                            Msg = "自爆失败！",
                            Time = DateTime.Now,
                            ToGroup = MsgDTO.FromGroup
                        });
                return;
            }

            MsgSender.Instance.PushMsg(
                new MsgCommand
                    {
                        Command = AiCommand.SendGroup,
                        Id = Guid.NewGuid().ToString(),
                        Msg = "AI即将自爆！",
                        Time = DateTime.Now,
                        ToGroup = MsgDTO.FromGroup
                    });
            for (var i = 5; i > 0; i--)
            {
                MsgSender.Instance.PushMsg(
                    new MsgCommand
                        {
                            Command = AiCommand.SendGroup,
                            Id = Guid.NewGuid().ToString(),
                            Msg = i.ToString(),
                            Time = DateTime.Now,
                            ToGroup = MsgDTO.FromGroup
                        });
                Thread.Sleep(1000);
            }

            MsgSender.Instance.PushMsg(
                new MsgCommand
                    {
                        Command = AiCommand.SendGroup,
                        Id = Guid.NewGuid().ToString(),
                        Msg = CodeApi.Code_Image("images/boom.jpg"),
                        Time = DateTime.Now,
                        ToGroup = MsgDTO.FromGroup
                    });

            MsgSender.Instance.PushMsg(
                new MsgCommand
                    {
                        Command = AiCommand.Restart,
                        Id = Guid.NewGuid().ToString()
                    });
        }

        public override void Work()
        {
            
        }
    }
}
