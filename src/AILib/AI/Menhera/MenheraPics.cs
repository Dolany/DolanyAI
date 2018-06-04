﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flexlive.CQP.Framework;
using System.IO;

namespace AILib
{
    [AI(Name = "MenheraPicsAI", Description = "AI for Showing Menhera Pics.", IsAvailable = true)]
    public class MenheraPics : AIBase
    {
        private string picFoldPath = "./Menhera/";

        public MenheraPics(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {

        }

        public override void Work()
        {
            
        }

        public override void OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            if(!MsgDTO.msg.ToLower().Contains("menhera"))
            {
                return;
            }
            SendRandPicToGroup(MsgDTO.fromGroup);
        }

        public override void OnPrivateMsgReceived(PrivateMsgDTO MsgDTO)
        {
            Common.SendMsgToDevelper($@"received msg: {MsgDTO.msg}");
            if (!MsgDTO.msg.ToLower().Contains("menhera"))
            {
                return;
            }
            SendRandPicToGroup(MsgDTO.fromQQ);
        }

        private void SendRandPicToGroup(long GroupNumber)
        {
            string[] picNames = PicNames();
            Random ran = new Random();
            string ranPic = picNames[ran.Next(picNames.Length)];

            string picCode = CQ.CQCode_Image(ranPic);
            CQ.SendGroupMessage(GroupNumber, picCode);
        }

        private string[] PicNames()
        {
            List<string> nameList = new List<string>();
            DirectoryInfo folder = new DirectoryInfo(picFoldPath);

            foreach (FileInfo file in folder.GetFiles("*.jpg"))
            {
                nameList.Add(file.FullName);
            }

            return nameList.ToArray();
        }
    }
}
