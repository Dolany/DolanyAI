using AILib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Flexlive.CQP.Framework.Utils;

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

        public List<AlertContentEntity> AllAlertInfos
        {
            get
            {
                var query = DbMgr.Query<AlertContentEntity>();
                return query?.ToList();
            }
        }

        private List<long> AvailableGroups
        {
            get
            {
                var query = DbMgr.Query<AlertRegistedGroupEntity>(a => bool.Parse(a.Available));
                if (query.IsNullOrEmpty())
                {
                    return null;
                }
                return query.Select(q => long.Parse(q.Content)).ToList();
            }
        }

        public HourAlertAI(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
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

        [EnterCommand(
            Command = "报时",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.成员,
            Description = "设定指定小时的报时内容",
            Syntax = " [目标小时] [报时内容]",
            Tag = "闹钟与报时",
            SyntaxChecker = "HourAlert"
            )]
        public void AlertSet(GroupMsgDTO MsgDTO, object[] param)
        {
            AlertContentEntity info = param[0] as AlertContentEntity;

            info.CreateTime = DateTime.Now;
            info.Creator = MsgDTO.fromQQ;
            info.FromGroup = MsgDTO.fromGroup;

            string Msg = SaveAlertContent(info) ? "报时内容保存成功！" : "报时内容保存失败！";
            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = Msg
            });
        }

        [EnterCommand(
            Command = "报时开启",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.管理员,
            Description = "设置报时功能开启",
            Syntax = "",
            Tag = "闹钟与报时",
            SyntaxChecker = "Empty"
            )]
        public void AlertEnable(GroupMsgDTO MsgDTO, object[] param)
        {
            RuntimeLogger.Log("HourAlertAI Tryto AlertEnable");
            AvailableStateChange(MsgDTO.fromGroup, true);
            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = "报时功能已开启！"
            });
            RuntimeLogger.Log("HourAlertAI AlertEnable Completed");
        }

        [EnterCommand(
            Command = "报时关闭",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.管理员,
            Description = "设置报时功能关闭",
            Syntax = "",
            Tag = "闹钟与报时",
            SyntaxChecker = "Empty"
            )]
        public void AlertDisenable(GroupMsgDTO MsgDTO, object[] param)
        {
            RuntimeLogger.Log("HourAlertAI Tryto AlertDisenable");
            AvailableStateChange(MsgDTO.fromGroup, false);
            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = "报时功能已关闭！"
            });
            RuntimeLogger.Log("HourAlertAI AlertDisenable Completed");
        }

        private void AvailableStateChange(long groupNumber, bool state)
        {
            var query = DbMgr.Query<AlertRegistedGroupEntity>(a => long.Parse(a.Content) == groupNumber);
            if (query.IsNullOrEmpty())
            {
                DbMgr.Insert(new AlertRegistedGroupEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = groupNumber.ToString(),
                    Available = state.ToString()
                });
            }
            else
            {
                var arg = query.FirstOrDefault();
                arg.Available = state.ToString();
                DbMgr.Update(arg);
            }
        }

        public void RecordAlertContent(string msg, long fromQQ, long fromGroup)
        {
            AlertContentEntity info = AlertContentEntity.Parse(msg);
            if (info == null)
            {
                return;
            }

            info.CreateTime = DateTime.Now;
            info.Creator = fromQQ;
            info.FromGroup = fromGroup;

            if (SaveAlertContent(info))
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO()
                {
                    Aim = fromGroup,
                    Type = MsgType.Group,
                    Msg = "报时内容保存成功！"
                });
            }
            else
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO()
                {
                    Aim = fromGroup,
                    Type = MsgType.Group,
                    Msg = "报时内容保存失败！"
                });
            }
        }

        private bool SaveAlertContent(AlertContentEntity info)
        {
            try
            {
                info.Id = Guid.NewGuid().ToString();
                DbMgr.Insert(info);

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
            var query = DbMgr.Query<AlertContentEntity>(a => a.FromGroup == fromGroup && a.AimHour == aimHour);
            if (query.IsNullOrEmpty())
            {
                return string.Empty;
            }
            var list = query.ToList();

            Random random = new Random();
            int randIdx = random.Next(list.Count);

            return list[randIdx].Content;
        }

        [EnterCommand(
            Command = "报时",
            SourceType = MsgType.Private,
            IsDeveloperOnly = true,
            Description = "获取指定群组和目标小时的随机报时内容",
            Syntax = " [目标群组] [目标小时]",
            Tag = "闹钟与报时",
            SyntaxChecker = "AlertPrivate"
            )]
        public void AlertPrivate(PrivateMsgDTO MsgDTO, object[] param)
        {
            long aimGroup = (long)param[0];
            int aimHour = (int)param[1];

            string RanContent = GetRanAlertContent(aimGroup, aimHour);
            Common.SendMsgToDeveloper($@"到{aimHour}点啦！ {RanContent}");
        }

        [EnterCommand(
            Command = "所有报时开启群组",
            SourceType = MsgType.Private,
            IsDeveloperOnly = true,
            Description = "获取所有报时开启群组的列表",
            Syntax = "",
            Tag = "闹钟与报时",
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

        [EnterCommand(
            Command = "清空报时",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.群主,
            Description = "清空指定小时的所有报时内容",
            Syntax = "[目标小时]",
            Tag = "闹钟与报时",
            SyntaxChecker = "Long"
            )]
        public void ClearAlert(GroupMsgDTO MsgDTO, object[] param)
        {
            long num = (long)param[0];

            if (num <= 24)
            {
                DbMgr.Delete<AlertContentEntity>(a => a.AimHour == (int)num);
            }
            else
            {
                DbMgr.Delete<AlertContentEntity>(a => a.Creator == num);
            }

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = "删除成功！"
            });
        }

        [EnterCommand(
            Command = "所有报时数目",
            SourceType = MsgType.Private,
            IsDeveloperOnly = true,
            Description = "获取所有的报时数目",
            Syntax = "",
            Tag = "闹钟与报时",
            SyntaxChecker = "Empty"
            )]
        public void TotalAlertCount(PrivateMsgDTO MsgDTO, object[] param)
        {
            Common.SendMsgToDeveloper(AllAlertInfos.Count().ToString());
        }
    }
}