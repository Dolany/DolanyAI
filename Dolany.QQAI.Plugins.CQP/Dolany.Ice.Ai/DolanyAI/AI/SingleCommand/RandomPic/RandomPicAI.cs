using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using static Dolany.Ice.Ai.MahuaApis.CodeApi;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(RandomPicAI),
        Description = "AI for Sending Random Pic By Keyword.",
        IsAvailable = true,
        PriorityLevel = 2
        )]
    public class RandomPicAI : AIBase
    {
        private string PicPath { get; } = ImagePath;
        private static List<string> Keywords => new List<string>();

        public RandomPicAI()
        {
            RuntimeLogger.Log("RandomPicAI started.");
        }

        public override void Work()
        {
            ReloadAllKeywords();
        }

        private void ReloadAllKeywords()
        {
            Keywords.Clear();

            var dirInfo = new DirectoryInfo(PicPath);
            var childrenDirs = dirInfo.GetDirectories();
            foreach (var info in childrenDirs)
            {
                Keywords.Add(info.Name);
                Consolers.Add(new EnterCommandAttribute
                {
                    AuthorityLevel = AuthorityLevel.成员,
                    SyntaxChecker = "Empty",
                    Command = info.Name
                }, KeywordsConsoler);
            }
        }

        private void KeywordsConsoler(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var RandPic = GetRandPic(MsgDTO.Command);
            if (string.IsNullOrEmpty(RandPic))
            {
                return;
            }

            var pic = new FileInfo(PicPath + MsgDTO.Command + "\\" + RandPic).FullName;
            MsgSender.Instance.PushMsg(MsgDTO, pic);
        }

        [EnterCommand(
            Command = "随机图片 一键盗图",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "随机发送近期内所有群组内发过的图片",
            Syntax = "",
            Tag = "图片功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailabe = true
            )]
        public void RecentPic(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var imageList = GetRecentImageList();
            var idx = Utility.RandInt(imageList.Count);
            var ImageCache = Utility.ReadImageCacheInfo(imageList[idx]);
            var sendImgName = $"{ImageCache.guid}.{ImageCache.type}";

            MsgSender.Instance.PushMsg(MsgDTO, Code_Image(sendImgName));
        }

        [EnterCommand(
            Command = "随机闪照",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "随机发送近期内所有群组内发过的图片（以闪照的形式）",
            Syntax = "",
            Tag = "图片功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailabe = true
            )]
        public void RecentFlash(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var imageList = GetRecentImageList();
            var idx = Utility.RandInt(imageList.Count);
            var ImageCache = Utility.ReadImageCacheInfo(imageList[idx]);
            var sendImgName = $"{ImageCache.guid}.{ImageCache.type}";

            MsgSender.Instance.PushMsg(MsgDTO, Code_Flash(sendImgName));
        }

        private List<FileInfo> GetRecentImageList()
        {
            var dirInfo = new DirectoryInfo(PicPath);
            var files = dirInfo.GetFiles();
            return files.Where(f => f.Extension == ImageExtension)
                        .OrderBy(f => f.CreationTime)
                        .Skip(10)
                        .ToList();
        }

        private string GetRandPic(string dirName)
        {
            var dirInfo = new DirectoryInfo(PicPath + dirName);
            var fil = dirInfo.GetFiles();
            if (fil.IsNullOrEmpty())
            {
                return string.Empty;
            }

            var f = fil[Utility.RandInt(fil.Length)];
            return f.Name;
        }

        [EnterCommand(
            Command = "添加同义词",
            Description = "添加图片检索时的关键字",
            AuthorityLevel = AuthorityLevel.开发者,
            Syntax = "[目标词] [同义词]",
            Tag = "图片功能",
            SyntaxChecker = "TwoWords",
            IsPrivateAvailabe = true
            )]
        public void AppendSynonym(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var Keyword = param[0] as string;
            var Content = param[1] as string;

            DbMgr.Insert(new SynonymDicEntity
            {
                Id = Guid.NewGuid().ToString(),
                Keyword = Keyword,
                Content = Content
            });

            Utility.SendMsgToDeveloper("添加成功！");
        }
    }
}