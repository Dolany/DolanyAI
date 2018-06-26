﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib.Entities;
using System.IO;
using Flexlive.CQP.Framework;
using Flexlive.CQP.Framework.Utils;

namespace AILib
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

        public RandomPicAI(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
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

            string key = GenKey(MsgDTO.command + MsgDTO.msg);
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            string RandPic = GetRandPic(key);

            SendPic(key + "/" + RandPic, MsgDTO.fromGroup);
            return true;
        }

        [EnterCommand(
            Command = "随机图片",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.成员,
            Description = "随机发送近期内所有群组内发过的图片",
            Syntax = "",
            Tag = "图片"
            )]
        public void RecentPic(GroupMsgDTO MsgDTO)
        {
            RuntimeLogger.Log("RandomPicAI Tryto RecentPic.");
            var imageList = GetRecentImageList();
            int idx = (new Random()).Next(imageList.Count());
            string sendImgName = imageList[idx].Name.Replace(".cqimg", "");

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = CQ.CQCode_Image(sendImgName)
            });
            RuntimeLogger.Log("RandomPicAI RecentPic completed");
        }

        private List<FileInfo> GetRecentImageList()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(PicPath);
            var files = dirInfo.GetFiles();
            return files.Where(f => f.Extension == ".cqimg").ToList();
        }

        private string GenKey(string msg)
        {
            var keys = Keywords.Where(k => msg.Contains(k));
            if (keys != null && keys.Count() > 0)
            {
                return keys.FirstOrDefault();
            }

            var query = DbMgr.Query<SynonymDicEntity>(s => msg.Contains(s.Content));
            if (query == null || query.Count() == 0)
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
                Msg = CQ.CQCode_Image(picPath)
            });
        }

        private string GetRandPic(string dirName)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(PicPath + dirName);
            FileInfo[] fil = dirInfo.GetFiles();
            if (fil == null || fil.Count() == 0)
            {
                return string.Empty;
            }

            Random random = new Random();
            var f = fil[random.Next(fil.Length)];
            return f.Name;
        }

        [EnterCommand(
            Command = "重新加载图片",
            SourceType = MsgType.Private,
            IsDeveloperOnly = true,
            Description = "重新加载图片列表，刷新搜索关键字",
            Syntax = "",
            Tag = "图片"
            )]
        public void RefreshKeywords(PrivateMsgDTO MsgDTO)
        {
            ReloadAllKeywords();

            Common.SendMsgToDeveloper($"共加载了{Keywords.Count}个图片组");
        }

        [EnterCommand(
            Command = "添加同义词",
            SourceType = MsgType.Private,
            IsDeveloperOnly = true,
            Description = "添加图片检索时的关键字",
            Syntax = "[目标词] [同义词]",
            Tag = "图片"
            )]
        public void AppendSynonym(PrivateMsgDTO MsgDTO)
        {
            if (string.IsNullOrEmpty(MsgDTO.msg))
            {
                return;
            }

            string[] strs = MsgDTO.msg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (strs == null || strs.Length != 2)
            {
                return;
            }

            DbMgr.Insert(new SynonymDicEntity()
            {
                Id = Guid.NewGuid().ToString(),
                Keyword = strs[0],
                Content = strs[1]
            });

            Common.SendMsgToDeveloper("添加成功！");
        }

        [EnterCommand(
            Command = "所有图片关键词",
            SourceType = MsgType.Private,
            IsDeveloperOnly = true,
            Description = "获取所有图片关键字（不包括同义词）",
            Syntax = "",
            Tag = "图片"
            )]
        public void AllPicKeywords(PrivateMsgDTO MsgDTO)
        {
            string msg = string.Empty;
            foreach (var k in Keywords)
            {
                msg += k + '\r';
            }

            Common.SendMsgToDeveloper(msg);
        }
    }
}