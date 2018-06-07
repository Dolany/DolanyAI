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
        private string xmlFilePath = @"./AI/HourAlert/AlertContent.xml";

        System.Timers.Timer timer;

        public List<AlertInfo> AllAlertInfos
        {
            get
            {
                XElement root = XElement.Load(xmlFilePath);

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

            foreach (var groupNum in ConfigDTO.AimGroups)
            {
                string RanContent = GetRanAlertContent(infoList, groupNum, int.Parse(curHour));

                CQ.SendGroupMessage(groupNum, $@"到{curHour}点啦！ {RanContent}");
            }
        }

        public override void OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            base.OnGroupMsgReceived(MsgDTO);

            RecordAlertContent(MsgDTO.msg, MsgDTO.fromQQ, MsgDTO.fromGroup);
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
                XElement root = XElement.Load(xmlFilePath);
                XElement node = new XElement("Content", info.AlertContent);
                node.SetAttributeValue("FromGroup", info.FromGroup.ToString());
                node.SetAttributeValue("Creator", info.Creator.ToString());
                node.SetAttributeValue("CreateTime", info.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                node.SetAttributeValue("AimHour", info.AimHour.ToString());
                root.Add(node);
                root.Save(xmlFilePath);

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
    }
}
