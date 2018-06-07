using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Flexlive.CQP.Framework;

namespace AILib
{
    [AI(Name = "Publisher", Description = "AI for Publishing Developing Record.", IsAvailable = true)]
    public class Publisher : AIBase
    {
        private string xmlFilePath = @"./AI/Interaction/Publisher/PublishRecord.xml";

        public Publisher(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {

        }

        public override void Work()
        {

        }

        public override void OnPrivateMsgReceived(PrivateMsgDTO MsgDTO)
        {
            base.OnPrivateMsgReceived(MsgDTO);

            if(MsgDTO.fromQQ != Common.DeveloperNumber)
            {
                return;
            }

            if(MsgDTO.msg.StartsWith("PublishRec "))
            {
                PublishRec(MsgDTO.msg.Replace("PublishRec ", ""));
                return;
            }

            if(MsgDTO.msg.StartsWith("PublishTo "))
            {
                PublishTo(MsgDTO.msg.Replace("PublishTo ", ""));
                return;
            }
        }

        private void PublishRec(string msg)
        {
            string Index = DateTime.Now.ToString("yyyyMMddHHmmss");

            try
            {
                XElement root = XElement.Load(xmlFilePath);
                XElement node = new XElement("Record", msg);
                node.SetAttributeValue("Index", Index);
                node.SetAttributeValue("CreateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                root.AddFirst(node);
                root.Save(xmlFilePath);

                Common.SendMsgToDeveloper($@"记录成功！序号为：{Index}");
            }
            catch(Exception ex)
            {
                Common.SendMsgToDeveloper(ex);
            }
        }

        private void PublishTo(string msg)
        {
            string[] strs = msg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if(strs == null || strs.Length != 2)
            {
                return;
            }

            string Rec = GetRecByIndex(strs[1]);
            long groupNum;
            if(!long.TryParse(strs[0], out groupNum))
            {
                return;
            }
            if(groupNum == 0)
            {
                foreach(var group in ConfigDTO.AimGroups)
                {
                    CQ.SendGroupMessage(group, Rec);
                }
            }
            else
            {
                CQ.SendGroupMessage(groupNum, Rec);
            }
        }

        public string GetRecByIndex(string Index)
        {
            try
            {
                XElement root = XElement.Load(xmlFilePath);
                foreach (XElement node in root.Nodes())
                {
                    if(node.Attribute("Index").Value != Index)
                    {
                        continue;
                    }

                    return node.Value;
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
