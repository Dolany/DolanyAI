using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flexlive.CQP.Framework;
using AILib.AI.HourAlert;
using System.Xml.Linq;

namespace AILib
{
    [AI(Name = "HourAlertAI", Description = "AI for Hour Alert.", IsAvailable = true)]
    public class HourAlertAI : AIBase
    {
        private string xmlFilePath_Content = @"./AI/HourAlert/AlertContent.xml";
        private string xmlFilePath_Groups = @"./AI/HourAlert/AlertRegistedGroup.xml";

        System.Timers.Timer timer;

        public List<AlertInfo> AllAlertInfos
        {
            get
            {
                XElement root = XElement.Load(xmlFilePath_Content);

                List<AlertInfo> list = new List<AlertInfo>();
                foreach (XElement node in root.Nodes())
                {
                    AlertInfo s = new AlertInfo()
                    {
                        FromGroup = long.Parse(node.Attribute("FromGroup").Value),
                        Creator = long.Parse(node.Attribute("Creator").Value),
                        CreateTime = DateTime.Parse(node.Attribute("CreateTime").Value),
                        AimHour = int.Parse(node.Attribute("AimHour").Value),
                        AlertContent = node.Value
                    };

                    list.Add(s);
                }

                return list;
            }
        }

        private List<long> AvailableGroups
        {
            get
            {
                XElement root = XElement.Load(xmlFilePath_Groups);

                List<long> list = new List<long>();
                foreach (XElement node in root.Nodes())
                {
                    if (bool.Parse(node.Attribute("Available").Value))
                    {
                        list.Add(long.Parse(node.Value));
                    }
                }

                return list;
            }
        }

        public HourAlertAI(AIConfigDTO ConfigDTO)
            :base(ConfigDTO)
        {

        }

        public override void Work()
        {
            HourAlertFunc();
        }

        private void HourAlertFunc()
        {
            TimeSpan ts = GetNextHourSpan();
            timer = new System.Timers.Timer(ts.TotalMilliseconds);

            timer.AutoReset = false;
            timer.Enabled = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerUp);
        }

        private void TimerUp(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Stop();
            HourAlert(DateTime.Now.Hour.ToString());
            timer.Interval = GetNextHourSpan().TotalMilliseconds;
            timer.Start();
        }

        private TimeSpan GetNextHourSpan()
        {
            DateTime nextHour = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")).AddHours(1);
            return nextHour - DateTime.Now;
        }

        private void HourAlert(string curHour)
        {
            var infoList = AllAlertInfos;
            var availableList = AvailableGroups;

            foreach (var groupNum in ConfigDTO.AimGroups)
            {
                if(!availableList.Contains(groupNum))
                {
                    continue;
                }

                string RanContent = GetRanAlertContent(infoList, groupNum, int.Parse(curHour));
                CQ.SendGroupMessage(groupNum, $@"到{curHour}点啦！ {RanContent}");
            }
        }

        public override void OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            base.OnGroupMsgReceived(MsgDTO);

            if(AlertManagement(MsgDTO.msg, MsgDTO.fromQQ, MsgDTO.fromGroup))
            {
                return;
            }

            RecordAlertContent(MsgDTO.msg, MsgDTO.fromQQ, MsgDTO.fromGroup);
        }

        private bool AlertManagement(string msg, long fromQQ, long fromGroup)
        {
            if(string.IsNullOrEmpty(msg))
            {
                return false;
            }

            string authority = CQ.GetGroupMemberInfo(fromGroup, fromQQ).Authority;
            if(authority != "群主" && authority != "管理员")
            {
                return false;
            }

            if (msg == "报时 开启")
            {
                AvailableStateChange(fromGroup, true);
                CQ.SendGroupMessage(fromGroup, "报时功能已开启！");
                return true;
            }
            if(msg == "报时 关闭")
            {
                AvailableStateChange(fromGroup, false);
                CQ.SendGroupMessage(fromGroup, "报时功能已关闭！");
                return true;
            }


            return false;
        }

        private void AvailableStateChange(long groupNumber, bool state)
        {
            XElement root = XElement.Load(xmlFilePath_Groups);

            bool isExist = false;
            foreach (XElement node in root.Nodes())
            {
                if (long.Parse(node.Value) == groupNumber)
                {
                    isExist = true;
                    node.SetAttributeValue("Available", state.ToString());
                    break;
                }
            }
            if(!isExist)
            {
                XElement newNode = new XElement("Group", groupNumber.ToString());
                newNode.SetAttributeValue("Available", state.ToString());
                root.Add(newNode);
            }
            
            root.Save(xmlFilePath_Groups);
        }

        public void RecordAlertContent(string msg, long fromQQ, long fromGroup)
        {
            AlertInfo info = AlertInfo.Parse(msg);
            if(info == null)
            {
                return;
            }

            info.CreateTime = DateTime.Now;
            info.Creator = fromQQ;
            info.FromGroup = fromGroup;

            if(SaveAlertContent(info))
            {
                CQ.SendGroupMessage(fromGroup, "报时内容保存成功！");
            }
            else
            {
                CQ.SendGroupMessage(fromGroup, "报时内容保存失败！");
            }
        }

        private bool SaveAlertContent(AlertInfo info)
        {
            try
            {
                XElement root = XElement.Load(xmlFilePath_Content);
                XElement node = new XElement("Content", info.AlertContent);
                node.SetAttributeValue("FromGroup", info.FromGroup.ToString());
                node.SetAttributeValue("Creator", info.Creator.ToString());
                node.SetAttributeValue("CreateTime", info.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                node.SetAttributeValue("AimHour", info.AimHour.ToString());
                root.Add(node);
                root.Save(xmlFilePath_Content);

                return true;
            }
            catch (Exception ex)
            {
                Common.SendMsgToDeveloper(ex);
                return false;
            }
        }

        private string GetRanAlertContent(List<AlertInfo> infoList, long fromGroup, int aimHour)
        {
            try
            {
                if(infoList == null || infoList.Count == 0)
                {
                    return string.Empty;
                }
                var query = from info in infoList
                            where info.FromGroup == fromGroup && info.AimHour == aimHour
                            select info;
                if (query == null || query.Count() == 0)
                {
                    return string.Empty;
                }
                List<AlertInfo> list = query.ToList();

                Random random = new Random();
                int randIdx = random.Next(list.Count);

                return list[randIdx].AlertContent;
            }
            catch (Exception ex)
            {
                Common.SendMsgToDeveloper(ex);
                return string.Empty;
            }
        }

        public override void OnPrivateMsgReceived(PrivateMsgDTO MsgDTO)
        {
            base.OnPrivateMsgReceived(MsgDTO);

            if(!MsgDTO.msg.StartsWith("报时 "))
            {
                return;
            }

            string msg = MsgDTO.msg.Replace("报时 ", "");
            int aimHour = int.Parse(msg);
            var infoList = AllAlertInfos;

            string RanContent = GetRanAlertContent(infoList, 469652754, aimHour);
            Common.SendMsgToDeveloper($@"到{aimHour}点啦！ {RanContent}");
        }

        public override bool IsPrivateDeveloperOnly()
        {
            return true;
        }
    }
}
