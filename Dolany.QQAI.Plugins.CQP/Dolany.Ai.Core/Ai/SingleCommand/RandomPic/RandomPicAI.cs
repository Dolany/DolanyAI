namespace Dolany.Ai.Core.Ai.SingleCommand.RandomPic
{
    using System.Collections.Generic;
    using System.Linq;

    using Dolany.Ai.Core.Base;
    using Dolany.Ai.Core.Cache;
    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Db;
    using Dolany.Ai.Core.Entities;

    using static Dolany.Ai.Core.API.CodeApi;

    [AI(
        Name = nameof(RandomPicAI),
        Description = "AI for Sending Random Pic By Keyword.",
        IsAvailable = true,
        PriorityLevel = 2)]
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
            IsPrivateAvailabe = true)]
        public void RecentPic(MsgInformationEx MsgDTO, object[] param)
        {
            var imageList = GetRecentImageList().ToList();
            var idx = Utility.RandInt(imageList.Count);
            var picUrl = imageList.ElementAt(idx).Content;

            MsgSender.Instance.PushMsg(MsgDTO, Code_Image(picUrl));
        }

        [EnterCommand(
            Command = "随机闪照",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "随机发送近期内所有群组内发过的图片（以闪照的形式）",
            Syntax = "",
            Tag = "图片功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailabe = true)]
        public void RecentFlash(MsgInformationEx MsgDTO, object[] param)
        {
            var imageList = GetRecentImageList().ToList();
            var idx = Utility.RandInt(imageList.Count);
            var picUrl = imageList.ElementAt(idx).Content;

            MsgSender.Instance.PushMsg(MsgDTO, Code_Flash(picUrl));
        }

        private static IEnumerable<PicCacheEntity> GetRecentImageList()
        {
            var pics = DbMgr.Query<PicCacheEntity>();
            return pics;
        }
    }
}
