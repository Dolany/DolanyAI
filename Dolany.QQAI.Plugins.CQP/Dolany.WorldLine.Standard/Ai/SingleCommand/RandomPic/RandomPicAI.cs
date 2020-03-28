using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;

namespace Dolany.WorldLine.Standard.Ai.SingleCommand.RandomPic
{
    public class RandomPicAI : AIBase
    {
        public override string AIName { get; set; } = "随机图片";

        public override string Description { get; set; } = "AI for Sending Random Pic By Keyword.";

        public override AIPriority PriorityLevel { get;} = AIPriority.Normal;

        [EnterCommand(ID = "RandomPicAI_RecentPic",
            Command = "随机图片 一键盗图",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "随机发送近期内所有群组内发过的图片",
            Syntax = "",
            Tag = CmdTagEnum.娱乐功能,
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true,
            DailyLimit = 10)]
        public bool RecentPic(MsgInformationEx MsgDTO, object[] param)
        {
            var picUrl = PicCacher.Random();
            MsgSender.PushMsg(MsgDTO, CodeApi.Code_Image_Relational(picUrl));
            return true;
        }

        [EnterCommand(ID = "RandomPicAI_RecentFlash",
            Command = "随机闪照",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "随机发送近期内所有群组内发过的图片（以闪照的形式）",
            Syntax = "",
            Tag = CmdTagEnum.娱乐功能,
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true,
            DailyLimit = 10)]
        public bool RecentFlash(MsgInformationEx MsgDTO, object[] param)
        {
            var picUrl = PicCacher.Random();
            MsgSender.PushMsg(MsgDTO, CodeApi.Code_Flash_Relational(picUrl));
            return true;
        }
    }
}
