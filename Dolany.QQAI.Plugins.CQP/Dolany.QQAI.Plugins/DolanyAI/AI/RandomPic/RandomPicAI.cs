﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Dolany.QQAI.Plugins.Snow.MahuaApis.CQ;

namespace Dolany.QQAI.Plugins.DolanyAI
{
    [AI(
        Name = "RandomPicAI",
        Description = "AI for Sending Random Pic By Keyword.",
        IsAvailable = true,
        PriorityLevel = 2
        )]
    public class RandomPicAI : AIBase
    {
        private string PicPath = "data/image/";
        private List<string> Keywords = new List<string>();

        public RandomPicAI()
            : base()
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

            DirectoryInfo dirInfo = new DirectoryInfo(PicPath);
            DirectoryInfo[] childrenDirs = dirInfo.GetDirectories();
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

            string key = GenKey(MsgDTO.Command + MsgDTO.Msg);
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            string RandPic = GetRandPic(key);

            SendPic(key + "/" + RandPic, MsgDTO.FromGroup);
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
            RuntimeLogger.Log("RandomPicAI Tryto RecentPic.");
            var imageList = GetRecentImageList();
            int idx = (new Random()).Next(imageList.Count());
            string sendImgName = imageList[idx].Name.Replace(".cqimg", "");

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = CQCode.CQCode_Image(sendImgName)
            });
            RuntimeLogger.Log("RandomPicAI RecentPic completed");
        }

        private List<FileInfo> GetRecentImageList()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(PicPath);
            var files = dirInfo.GetFiles();
            return files.Where(f => f.Extension == ".cqimg")
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

            return query.FirstOrDefault().Keyword;
        }

        private void SendPic(string picPath, long group)
        {
            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = group,
                Type = MsgType.Group,
                Msg = CQCode.CQCode_Image(picPath)
            });
        }

        private string GetRandPic(string dirName)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(PicPath + dirName);
            FileInfo[] fil = dirInfo.GetFiles();
            if (fil.IsNullOrEmpty())
            {
                return string.Empty;
            }

            Random random = new Random();
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
            string Keyword = param[0] as string;
            string Content = param[1] as string;

            DbMgr.Insert(new SynonymDicEntity()
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
            string msg = string.Empty;
            foreach (var k in Keywords)
            {
                msg += k + '\r';
            }

            Utility.SendMsgToDeveloper(msg);
        }
    }
}