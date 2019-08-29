using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using System;

namespace Dolany.Ai.Core.Ai.SingleCommand.IceNews
{
    public class IceNewsAI : AIBase
    {
        public override string AIName { get; set; } = "冰冰新闻";
        public override string Description { get; set; } = "AI for ice news";
        public override int PriorityLevel { get; set; } = 10;
        public override bool Enable { get; set; } = false;

        private const string CachePath = "./images/News/";

        [EnterCommand(ID = "IceNewsAI_DialyNews",
            Command = "冰冰日报",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取最新冰冰日报内容",
            Syntax = "",
            Tag = "娱乐功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true,
            DailyLimit = 3)]
        public bool DialyNews(MsgInformationEx MsgDTO, object[] param)
        {
            var lastNews = NewsMgr.Instance.LastNews;
            if (string.IsNullOrEmpty(lastNews))
            {
                MsgSender.PushMsg(MsgDTO, "抱歉，尚未读取到任何新闻！");
                return false;
            }

            MsgSender.PushMsg(MsgDTO, CodeApi.Code_Image_Relational(CachePath + lastNews));
            return true;
        }

        [EnterCommand(ID = "IceNewsAI_NewsContribute",
            Command = "新闻投稿",
            AuthorityLevel = AuthorityLevel.开发者,
            Description = "向冰冰日报投稿",
            Syntax = "",
            Tag = "娱乐功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true)]
        public bool NewsContribute(MsgInformationEx MsgDTO, object[] param)
        {
            var info = Waiter.Instance.WaitForInformation(MsgDTO, "请上传图片（不能超过300KB）！",
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

            var fileName = $"{DateTime.Now:yyyyMMddHHmmss}.{imageCache.type}";
            if (!Utility.DownloadImage(imageCache.url, CachePath + fileName))
            {
                MsgSender.PushMsg(MsgDTO, "图片下载失败，请稍后再试！");
                return false;
            }
            NewsMgr.Instance.AddNews(fileName);

            MsgSender.PushMsg(MsgDTO, "保存成功！");
            return true;
        }
    }
}
