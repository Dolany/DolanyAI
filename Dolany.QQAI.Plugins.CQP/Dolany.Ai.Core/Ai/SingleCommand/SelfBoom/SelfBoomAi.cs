namespace Dolany.Ai.Core.Ai.SingleCommand.SelfBoom
{
    using System.Threading;

    using API;
    using Base;
    using Cache;
    using Common;
    using Model;
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
            IsPrivateAvailable = false)]
        public void Boom(MsgInformationEx MsgDTO, object[] param)
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
                return;
            }

            Thread.Sleep(1000);
            MsgSender.Instance.PushMsg(MsgDTO, "AI即将自爆！");
            for (var i = 5; i > 0; i--)
            {
                MsgSender.Instance.PushMsg(MsgDTO, i.ToString());
                Thread.Sleep(1000);
            }

            MsgSender.Instance.PushMsg(MsgDTO, CodeApi.Code_Image_Relational("images/boom.jpg"));

            BoomCode = Utility.RandInt(10000);
            Thread.Sleep(1000);
            MsgSender.Instance.PushMsg(
                new MsgCommand
                    {
                        Command = AiCommand.Restart
                    });
        }

        [EnterCommand(
            Command = "BoomCode",
            AuthorityLevel = AuthorityLevel.开发者,
            Description = "获取自爆指令码",
            Syntax = "",
            Tag = "系统功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true)]
        public void GetBoomCode(MsgInformationEx MsgDTO, object[] param)
        {
            MsgSender.Instance.PushMsg(MsgDTO, BoomCode.ToString());
        }

        public override void Initialization()
        {
        }
    }
}
