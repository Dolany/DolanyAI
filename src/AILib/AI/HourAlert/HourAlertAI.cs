/*已迁移*/

using AILib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Flexlive.CQP.Framework.Utils;
using System.ComponentModel.Composition;
using AILib.Db;

namespace AILib
{
    [AI(
        Name = "HourAlertAI",
        Description = "AI for Hour Alert.",
        IsAvailable = true,
        PriorityLevel = 10
        )]
    public class HourAlertAI : AIBase
    {
        private System.Timers.Timer timer;

        public List<AlertContent> AllAlertInfos
        {
            get
            {
                using (AIDatabase db = new AIDatabase())
                {
                    var query = db.AlertContent;
                    return query?.ToList();
                }
            }
        }

        private List<long> AvailableGroups
        {
            get
            {
                using (AIDatabase db = new AIDatabase())
                {
                    var query = db.AlertRegistedGroup.Where(a => bool.Parse(a.Available));
                    if (query.IsNullOrEmpty())
                    {
                        return null;
                    }
                    return query.Select(q => q.GroupNum).ToList();
                }
            }
        }

        public HourAlertAI()
            : base()
        {
            RuntimeLogger.Log("HourAlertAI started");
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
            timer.Elapsed += new System.Timers.ElapsedEventHandler(TimeUp);

            timer.Start();
        }

        private void TimeUp(object sender, System.Timers.ElapsedEventArgs e)
        {
            RuntimeLogger.Log("HourAlertAI TimeUp");
            timer.Stop();
            System.Threading.Thread.Sleep(3 * 1000);
            HourAlert(DateTime.Now.Hour);
            timer.Interval = GetNextHourSpan().TotalMilliseconds;
            timer.Start();
            RuntimeLogger.Log("HourAlertAI TimeUp Completed");
        }

        private TimeSpan GetNextHourSpan()
        {
            DateTime nextHour = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")).AddHours(1);
            return nextHour - DateTime.Now;
        }

        private void HourAlert(int curHour)
        {
            RuntimeLogger.Log("HourAlertAI HourAlert");
            var availableList = AvailableGroups;
            if (availableList.IsNullOrEmpty())
            {
                return;
            }

            foreach (var groupNum in availableList)
            {
                string RanContent = GetRanAlertContent(groupNum, curHour);
                MsgSender.Instance.PushMsg(new SendMsgDTO()
                {
                    Aim = groupNum,
                    Type = MsgType.Group,
                    Msg = $@"到{curHour}点啦！ {RanContent}"
                });
            }
            RuntimeLogger.Log("HourAlertAI HourAlert Completed");
        }

        [GroupEnterCommand(
            Command = "报时",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "设定指定小时的报时内容",
            Syntax = " [目标小时] [报时内容]",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "HourAlert"
            )]
        public void AlertSet(GroupMsgDTO MsgDTO, object[] param)
        {
            AlertContent info = param[0] as AlertContent;

            info.CreateTime = DateTime.Now;
            info.Creator = MsgDTO.FromQQ;
            info.FromGroup = MsgDTO.FromGroup;

            string Msg = SaveAlertContent(info) ? "报时内容保存成功！" : "报时内容保存失败！";
            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = Msg
            });
        }

        [GroupEnterCommand(
            Command = "报时开启",
            AuthorityLevel = AuthorityLevel.管理员,
            Description = "设置报时功能开启",
            Syntax = "",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "Empty"
            )]
        public void AlertEnable(GroupMsgDTO MsgDTO, object[] param)
        {
            RuntimeLogger.Log("HourAlertAI Tryto AlertEnable");
            AvailableStateChange(MsgDTO.FromGroup, true);
            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = "报时功能已开启！"
            });
            RuntimeLogger.Log("HourAlertAI AlertEnable Completed");
        }

        [GroupEnterCommand(
            Command = "报时关闭",
            AuthorityLevel = AuthorityLevel.管理员,
            Description = "设置报时功能关闭",
            Syntax = "",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "Empty"
            )]
        public void AlertDisenable(GroupMsgDTO MsgDTO, object[] param)
        {
            RuntimeLogger.Log("HourAlertAI Tryto AlertDisenable");
            AvailableStateChange(MsgDTO.FromGroup, false);
            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = "报时功能已关闭！"
            });
            RuntimeLogger.Log("HourAlertAI AlertDisenable Completed");
        }

        private void AvailableStateChange(long groupNumber, bool state)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.AlertRegistedGroup.Where(a => a.GroupNum == groupNumber);
                if (query.IsNullOrEmpty())
                {
                    db.AlertRegistedGroup.Add(new AlertRegistedGroup()
                    {
                        Id = Guid.NewGuid().ToString(),
                        GroupNum = groupNumber,
                        Available = state.ToString()
                    });
                }
                else
                {
                    var arg = query.FirstOrDefault();
                    arg.Available = state.ToString();
                }
                db.SaveChanges();
            }
        }

        private bool SaveAlertContent(AlertContent info)
        {
            try
            {
                using (AIDatabase db = new AIDatabase())
                {
                    info.Id = Guid.NewGuid().ToString();
                    db.AlertContent.Add(info);
                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                Common.SendMsgToDeveloper(ex);
                return false;
            }
        }

        private string GetRanAlertContent(long fromGroup, int aimHour)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.AlertContent.Where(a => a.FromGroup == fromGroup && a.AimHour == aimHour);
                if (query.IsNullOrEmpty())
                {
                    return string.Empty;
                }
                var list = query.ToList();

                Random random = new Random();
                int randIdx = random.Next(list.Count);

                return list[randIdx].Content;
            }
        }

        [PrivateEnterCommand(
            Command = "报时",
            Description = "获取指定群组和目标小时的随机报时内容",
            Syntax = " [目标群组] [目标小时]",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "AlertPrivate"
            )]
        public void AlertPrivate(PrivateMsgDTO MsgDTO, object[] param)
        {
            long aimGroup = (long)param[0];
            int aimHour = (int)param[1];

            string RanContent = GetRanAlertContent(aimGroup, aimHour);
            Common.SendMsgToDeveloper($@"到{aimHour}点啦！ {RanContent}");
        }

        [PrivateEnterCommand(
            Command = "所有报时开启群组",
            Description = "获取所有报时开启群组的列表",
            Syntax = "",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "Empty"
            )]
        public void AllAvailabeGroups(PrivateMsgDTO MsgDTO, object[] param)
        {
            var list = AvailableGroups;
            string msg = $"共有群组{list.Count}个";
            foreach (var l in list)
            {
                msg += '\r' + l.ToString();
            }

            Common.SendMsgToDeveloper(msg);
        }

        [GroupEnterCommand(
            Command = "清空报时",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "清空指定小时的所有报时内容",
            Syntax = "[目标小时]",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "Long"
            )]
        public void ClearAlert(GroupMsgDTO MsgDTO, object[] param)
        {
            long num = (long)param[0];

            using (AIDatabase db = new AIDatabase())
            {
                if (num <= 24)
                {
                    var query = db.AlertContent.Where(a => a.AimHour == (int)num);
                    db.AlertContent.RemoveRange(query);
                }
                else
                {
                    var query = db.AlertContent.Where(a => a.Creator == num);
                    db.AlertContent.RemoveRange(query);
                }

                db.SaveChanges();
            }

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = "删除成功！"
            });
        }

        [PrivateEnterCommand(
            Command = "所有报时数目",
            Description = "获取所有的报时数目",
            Syntax = "",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "Empty"
            )]
        public void TotalAlertCount(PrivateMsgDTO MsgDTO, object[] param)
        {
            Common.SendMsgToDeveloper(AllAlertInfos.Count().ToString());
        }
    }
}