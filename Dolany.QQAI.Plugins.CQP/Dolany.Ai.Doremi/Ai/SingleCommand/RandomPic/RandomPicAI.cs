using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Doremi.Base;
using Dolany.Ai.Doremi.Cache;

namespace Dolany.Ai.Doremi.Ai.SingleCommand.RandomPic
{
    [AI(Name = "随机图片",
        Description = "AI for Sending Random Pic By Keyword.",
        Enable = true,
        PriorityLevel = 10,
        BindAi = "Doremi")]
    public class RandomPicAI : AIBase
    {
        [EnterCommand(ID = "RandomPicAI_RecentPic",
            Command = "随机图片 一键盗图",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "随机发送近期内所有群组内发过的图片",
            Syntax = "",
            Tag = "娱乐功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true,
            DailyLimit = 10)]
        public bool RecentPic(MsgInformationEx MsgDTO, object[] param)
        {
            var picUrl = PicCacher.Random();
            MsgSender.PushMsg(MsgDTO, CodeApi.Code_Image(picUrl));
            return true;
        }

        [EnterCommand(ID = "RandomPicAI_RecentFlash",
            Command = "随机闪照",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "随机发送近期内所有群组内发过的图片（以闪照的形式）",
            Syntax = "",
            Tag = "娱乐功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true,
            DailyLimit = 10)]
        public bool RecentFlash(MsgInformationEx MsgDTO, object[] param)
        {
            var picUrl = PicCacher.Random();
            MsgSender.PushMsg(MsgDTO, CodeApi.Code_Flash(picUrl));
            return true;
        }
    }
}
