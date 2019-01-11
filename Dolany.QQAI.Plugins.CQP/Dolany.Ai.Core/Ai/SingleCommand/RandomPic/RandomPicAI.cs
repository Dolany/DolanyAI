namespace Dolany.Ai.Core.Ai.SingleCommand.RandomPic
{
    using Dolany.Ai.Core.Base;
    using Dolany.Ai.Core.Cache;
    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Model;

    using static Dolany.Ai.Core.API.CodeApi;

    [AI(
        Name = nameof(RandomPicAI),
        Description = "AI for Sending Random Pic By Keyword.",
        IsAvailable = true,
        PriorityLevel = 10)]
    public class RandomPicAI : AIBase
    {
        public RandomPicAI()
        {
            RuntimeLogger.Log("RandomPicAI started.");
        }

        public override void Work()
        {
        }

        [EnterCommand(
            Command = "随机图片 一键盗图",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "随机发送近期内所有群组内发过的图片",
            Syntax = "",
            Tag = "图片功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true)]
        public void RecentPic(MsgInformationEx MsgDTO, object[] param)
        {
            var picUrl = PicCacher.Random();

            MsgSender.Instance.PushMsg(MsgDTO, Code_Image(picUrl));
        }

        [EnterCommand(
            Command = "随机闪照",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "随机发送近期内所有群组内发过的图片（以闪照的形式）",
            Syntax = "",
            Tag = "图片功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true)]
        public void RecentFlash(MsgInformationEx MsgDTO, object[] param)
        {
            var picUrl = PicCacher.Random();

            MsgSender.Instance.PushMsg(MsgDTO, Code_Flash(picUrl));
        }
    }
}
