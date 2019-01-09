namespace Dolany.Ai.Core.Ai.SingleCommand.SelfBoom
{
    using System;
    using System.IO;
    using System.Threading;

    using Dolany.Ai.Core.API;
    using Dolany.Ai.Core.Base;
    using Dolany.Ai.Core.Cache;
    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Model;
    using Dolany.Database.Ai;

    [AI(
        Name = nameof(SelfBoomAi),
        Description = "AI for boom herself.",
        IsAvailable = true,
        PriorityLevel = 10)]
    public class SelfBoomAi : AIBase
    {
        private int BoomCode = Utility.RandInt(10000);

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
                info => info.Msg == BoomCode.ToString());
            if (backInfo == null)
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

            Thread.Sleep(1000);
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
                        Msg = CodeApi.Code_Image(new FileInfo("images/boom.jpg").FullName),
                        Time = DateTime.Now,
                        ToGroup = MsgDTO.FromGroup
                    });

            BoomCode = Utility.RandInt(10000);
            Thread.Sleep(1000);
            MsgSender.Instance.PushMsg(
                new MsgCommand
                    {
                        Command = AiCommand.Restart,
                        Id = Guid.NewGuid().ToString(),
                        Time = DateTime.Now
                    });
        }

        [EnterCommand(
            Command = "BoomCode",
            AuthorityLevel = AuthorityLevel.开发者,
            Description = "获取自爆指令码",
            Syntax = "",
            Tag = "系统功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailabe = true)]
        public void GetBoomCode(MsgInformationEx MsgDTO, object[] param)
        {
            MsgSender.Instance.PushMsg(
                new MsgCommand
                    {
                        Command = AiCommand.SendGroup,
                        Id = Guid.NewGuid().ToString(),
                        Msg = BoomCode.ToString(),
                        Time = DateTime.Now,
                        ToGroup = MsgDTO.FromGroup
                    });
        }

        public override void Work()
        {
        }
    }
}
