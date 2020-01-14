using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using System;

namespace Dolany.WorldLine.Standard.Ai.SingleCommand.IceNews
{
    public class ArknightsAI : AIBase
    {
        public override string AIName { get; set; } = "明日方舟";
        public override string Description { get; set; } = "AI for Arknights.";
        public override int PriorityLevel { get; set; } = 10;

        private const string CachePath = "./images/Arknights/";

        [EnterCommand(ID = "ArknightsAI_MiniStory",
            Command = "方舟剧场",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取最新方舟剧场内容",
            Syntax = "",
            Tag = "娱乐功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true,
            DailyLimit = 3)]
        public bool MiniStory(MsgInformationEx MsgDTO, object[] param)
        {
            var rec = ArknightsMiniStoryRecord.GetLast();

            var msg = $"【No.{rec.No}】\r{CodeApi.Code_Image_Relational(CachePath + rec.Path)}";
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "ArknightsAI_MiniStoryIndex",
            Command = "方舟剧场",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "按期号获取最新方舟剧场内容",
            Syntax = "[期号]",
            Tag = "娱乐功能",
            SyntaxChecker = "Long",
            IsPrivateAvailable = true,
            DailyLimit = 3)]
        public bool MiniStoryIndex(MsgInformationEx MsgDTO, object[] param)
        {
            var no = (int)(long)param[0];
            var rec = ArknightsMiniStoryRecord.Get(no);
            if (rec == null)
            {
                MsgSender.PushMsg(MsgDTO, "未找到相应的期号！");
                return false;
            }

            var msg = $"【No.{rec.No}】\r{CodeApi.Code_Image_Relational(CachePath + rec.Path)}";
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "IceNewsAI_StoryContribute",
            Command = "方舟剧场投稿",
            AuthorityLevel = AuthorityLevel.开发者,
            Description = "向方舟剧场投稿",
            Syntax = "",
            Tag = "娱乐功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true)]
        public bool StoryContribute(MsgInformationEx MsgDTO, object[] param)
        {
            var info = Waiter.Instance.WaitForInformation(MsgDTO, "请上传图片！",
                information => information.FromGroup == MsgDTO.FromGroup && information.FromQQ == MsgDTO.FromQQ &&
                               !string.IsNullOrEmpty(Utility.ParsePicGuid(information.Msg)), 10);
            if (info == null)
            {
                MsgSender.PushMsg(MsgDTO, "操作取消!");
                return false;
            }

            var bindai = BindAiMgr.Instance[MsgDTO.BindAi];

            var picGuid = Utility.ParsePicGuid(info.Msg);
            var imageCache = Utility.ReadImageCacheInfo(picGuid, bindai.ImagePath);
            if (imageCache == null)
            {
                MsgSender.PushMsg(MsgDTO, "文件缓存读取失败！");
                return false;
            }

            var fileName = $"MiniStory{DateTime.Now:yyyyMMddHHmmss}.{imageCache.type}";
            if (!Utility.DownloadImage(imageCache.url, CachePath + fileName))
            {
                MsgSender.PushMsg(MsgDTO, "图片下载失败，请稍后再试！");
                return false;
            }
            ArknightsMiniStoryRecord.Insert(fileName);

            MsgSender.PushMsg(MsgDTO, "保存成功！");
            return true;
        }
    }
}
