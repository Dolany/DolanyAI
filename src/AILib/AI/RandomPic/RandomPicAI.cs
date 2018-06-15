using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib.Entities;
using System.IO;
using Flexlive.CQP.Framework;

namespace AILib
{
    [AI(Name = "RandomPicAI", Description = "AI for Sending Random Pic By Keyword.", IsAvailable = true)]
    public class RandomPicAI : AIBase
    {
        private string PicPath = "data/image/";
        private List<string> Keywords = new List<string>();

        public RandomPicAI(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {

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
            foreach(var info in childrenDirs)
            {
                Keywords.Add(info.Name);
            }
        }

        public override void OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            string key = GenKey(MsgDTO.command + MsgDTO.msg);
            if(string.IsNullOrEmpty(key))
            {
                return;
            }

            string RandPic = GetRandPic(key);

            SendPic(key + "/" + RandPic, MsgDTO.fromGroup);
        }

        private string GenKey(string msg)
        {
            var keys = Keywords.Where(k => msg.Contains(k));
            if (keys != null && keys.Count() > 0)
            {
                return keys.FirstOrDefault();
            }

            var query = DbMgr.Query<SynonymDicEntity>(s => msg.Contains(s.Content));
            if(query == null || query.Count() == 0)
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
            if(fil == null || fil.Count() == 0)
            {
                return string.Empty;
            }

            Random random = new Random();
            var f = fil[random.Next(fil.Length)];
            return f.Name;
        }

        [EnterCommand(Command = "重新加载图片", SourceType = MsgType.Private, IsDeveloperOnly = true)]
        public void RefreshKeywords(PrivateMsgDTO MsgDTO)
        {
            ReloadAllKeywords();

            Common.SendMsgToDeveloper($"共加载了{Keywords.Count}个图片组");
        }

        [EnterCommand(Command = "添加同义词", SourceType = MsgType.Private, IsDeveloperOnly = true)]
        public void AppendSynonym(PrivateMsgDTO MsgDTO)
        {
            if(string.IsNullOrEmpty(MsgDTO.msg))
            {
                return;
            }

            string[] strs = MsgDTO.msg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if(strs == null || strs.Length != 2)
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
    }
}
