﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Dolany.Ice.Ai.MahuaApis;

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
        private string PicPath { get; set; } = CodeApi.ImagePath;
        private List<string> Keywords { get; set; } = new List<string>();

        public RandomPicAI()
        {
            RuntimeLogger.Log("RandomPicAI started.");
        }

        public override void Work()
        {
            ReloadAllKeywords();
        }

        public void ReloadAllKeywords()
        {
            Keywords.Clear();

            var dirInfo = new DirectoryInfo(PicPath);
            var childrenDirs = dirInfo.GetDirectories();
            foreach (var info in childrenDirs)
            {
                Keywords.Add(info.Name);
            }
        }

        public override bool OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            if (base.OnGroupMsgReceived(MsgDTO))
            {
                return true;
            }

            var key = GenKey(MsgDTO.Command + MsgDTO.Msg);
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            var RandPic = GetRandPic(key);

            SendPic(Environment.CurrentDirectory + "/" + PicPath + key + "/" + RandPic, MsgDTO.FromGroup);
            return true;
        }

        [GroupEnterCommand(
            Command = "随机图片",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "随机发送近期内所有群组内发过的图片",
            Syntax = "",
            Tag = "图片功能",
            SyntaxChecker = "Empty"
            )]
        [GroupEnterCommand(
            Command = "一键盗图",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "随机发送近期内所有群组内发过的图片",
            Syntax = "",
            Tag = "图片功能",
            SyntaxChecker = "Empty"
            )]
        public void RecentPic(GroupMsgDTO MsgDTO, object[] param)
        {
            var imageList = GetRecentImageList();
            var idx = (new Random()).Next(imageList.Count());
            var ImageCache = Utility.ReadImageCacheInfo(imageList[idx]);
            var sendImgName = $"{ImageCache.guid}.{ImageCache.type}";

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = CodeApi.Code_Image(sendImgName)
            });
        }

        [GroupEnterCommand(
            Command = "随机闪照",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "随机发送近期内所有群组内发过的图片（以闪照的形式）",
            Syntax = "",
            Tag = "图片功能",
            SyntaxChecker = "Empty"
            )]
        public void RecentFlash(GroupMsgDTO MsgDTO, object[] param)
        {
            var imageList = GetRecentImageList();
            var idx = (new Random()).Next(imageList.Count());
            var ImageCache = Utility.ReadImageCacheInfo(imageList[idx]);
            var sendImgName = $"{ImageCache.guid}.{ImageCache.type}";

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = CodeApi.Code_Flash(sendImgName)
            });
        }

        private List<FileInfo> GetRecentImageList()
        {
            var dirInfo = new DirectoryInfo(PicPath);
            var files = dirInfo.GetFiles();
            return files.Where(f => f.Extension == CodeApi.ImageExtension)
                        .OrderBy(f => f.CreationTime)
                        .Skip(10)
                        .ToList();
        }

        private string GenKey(string msg)
        {
            var keys = Keywords.Where(k => msg == k);
            if (!keys.IsNullOrEmpty())
            {
                return keys.FirstOrDefault();
            }

            var query = DbMgr.Query<SynonymDicEntity>(s => msg == s.Content);
            if (query.IsNullOrEmpty())
            {
                return string.Empty;
            }

            return query.FirstOrDefault()?.Keyword;
        }

        private void SendPic(string picPath, long group)
        {
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = group,
                Type = MsgType.Group,
                Msg = CodeApi.Code_Image(picPath)
            });
        }

        private string GetRandPic(string dirName)
        {
            var dirInfo = new DirectoryInfo(PicPath + dirName);
            var fil = dirInfo.GetFiles();
            if (fil.IsNullOrEmpty())
            {
                return string.Empty;
            }

            var random = new Random();
            var f = fil[random.Next(fil.Length)];

            return f.Name;
        }

        [PrivateEnterCommand(
            Command = "重新加载图片",
            Description = "重新加载图片列表，刷新搜索关键字",
            Syntax = "",
            Tag = "图片功能",
            SyntaxChecker = "Empty"
            )]
        public void RefreshKeywords(PrivateMsgDTO MsgDTO, object[] param)
        {
            ReloadAllKeywords();

            Utility.SendMsgToDeveloper($"共加载了{Keywords.Count}个图片组");
        }

        [PrivateEnterCommand(
            Command = "添加同义词",
            Description = "添加图片检索时的关键字",
            Syntax = "[目标词] [同义词]",
            Tag = "图片功能",
            SyntaxChecker = "TwoWords"
            )]
        public void AppendSynonym(PrivateMsgDTO MsgDTO, object[] param)
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

        [PrivateEnterCommand(
            Command = "所有图片关键词",
            Description = "获取所有图片关键字（不包括同义词）",
            Syntax = "",
            Tag = "图片功能",
            SyntaxChecker = "Empty"
            )]
        public void AllPicKeywords(PrivateMsgDTO MsgDTO, object[] param)
        {
            var msg = string.Empty;
            var builder = new StringBuilder();
            builder.Append(msg);
            foreach (var k in Keywords)
            {
                builder.Append(k + '\r');
            }
            msg = builder.ToString();

            Utility.SendMsgToDeveloper(msg);
        }
    }
}