using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;

namespace Dolany.WorldLine.Doremi.Ai.SingleCommand.RandomPic
{
    public class RandomPicAI : AIBase
    {
        public override string AIName { get; set; } = "随机图片";
        public override string Description { get; set; } = "AI for Sending Random Pic By Keyword.";
        protected override CmdTagEnum DefaultTag { get; } = CmdTagEnum.娱乐功能;

        [EnterCommand(ID = "RandomPicAI_RecentPic",
            Command = "随机图片 一键盗图",
            Description = "随机发送近期内所有群组内发过的图片",
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
            Description = "随机发送近期内所有群组内发过的图片（以闪照的形式）",
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
