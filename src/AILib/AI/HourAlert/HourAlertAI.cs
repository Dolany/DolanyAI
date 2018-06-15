using AILib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AILib
{
    [AI(Name = "HourAlertAI", Description = "AI for Hour Alert.", IsAvailable = true)]
    public class HourAlertAI : AIBase
    {
        private System.Timers.Timer timer;

        public List<AlertContentEntity> AllAlertInfos
        {
            get
            {
                var query = DbMgr.Query<AlertContentEntity>();
                return query == null ? null : query.ToList();
            }
        }

        private List<long> AvailableGroups
        {
            get
            {
                var query = DbMgr.Query<AlertRegistedGroupEntity>(a => bool.Parse(a.Available));
                if (query == null)
                {
                    return null;
                }
                return query.Select(q => long.Parse(q.Content)).ToList();
            }
        }

        public HourAlertAI(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
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
            timer.Elapsed += new System.Timers.ElapsedEventHandler(TimeUp);
        }

        private void TimeUp(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Stop();
            System.Threading.Thread.Sleep(3 * 1000);
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
            var availableList = AvailableGroups;
            if (availableList == null || availableList.Count() == 0)
            {
                return;
            }

            try
            {
                foreach (var groupNum in availableList)
                {
                    string RanContent = GetRanAlertContent(groupNum, int.Parse(curHour));
                    MsgSender.Instance.PushMsg(new SendMsgDTO()
                    {
                        Aim = groupNum,
                        Type = MsgType.Group,
                        Msg = $@"到{curHour}点啦！ {RanContent}"
                    });
                }
            }
            catch (Exception ex)
            {
                Common.SendMsgToDeveloper(ex);
            }
        }

        [EnterCommand(Command = "报时", SourceType = MsgType.Group, AuthorityLevel = AuthorityLevel.成员)]
        public void AlertSet(GroupMsgDTO MsgDTO)
        {
            RecordAlertContent(MsgDTO.msg, MsgDTO.fromQQ, MsgDTO.fromGroup);
        }

        [EnterCommand(Command = "报时开启", SourceType = MsgType.Group, AuthorityLevel = AuthorityLevel.管理员)]
        public void AlertEnable(GroupMsgDTO MsgDTO)
        {
            AvailableStateChange(MsgDTO.fromGroup, true);
            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = "报时功能已开启！"
            });
        }

        [EnterCommand(Command = "报时关闭", SourceType = MsgType.Group, AuthorityLevel = AuthorityLevel.管理员)]
        public void AlertDisenable(GroupMsgDTO MsgDTO)
        {
            AvailableStateChange(MsgDTO.fromGroup, false);
            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = "报时功能已关闭！"
            });
        }

        private void AvailableStateChange(long groupNumber, bool state)
        {
            var query = DbMgr.Query<AlertRegistedGroupEntity>(a => long.Parse(a.Content) == groupNumber);
            if (query == null || query.Count() == 0)
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
            if (query == null || query.Count() == 0)
            {
                return string.Empty;
            }
            var list = query.ToList();

            Random random = new Random();
            int randIdx = random.Next(list.Count);

            return list[randIdx].Content;
        }

        [EnterCommand(Command = "报时", SourceType = MsgType.Private, IsDeveloperOnly = true)]
        public void AlertPrivate(PrivateMsgDTO MsgDTO)
        {
            string[] strs = MsgDTO.msg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (strs == null || strs.Length != 2)
            {
                return;
            }

            int aimHour;
            if (!int.TryParse(strs[0], out aimHour))
            {
                return;
            }

            long aimGroup;
            if (!long.TryParse(strs[1], out aimGroup))
            {
                return;
            }

            string RanContent = GetRanAlertContent(aimGroup, aimHour);
            Common.SendMsgToDeveloper($@"到{aimHour}点啦！ {RanContent}");
        }

        [EnterCommand(Command = "所有报时开启群组", SourceType = MsgType.Private, IsDeveloperOnly = true)]
        public void AllAvailabeGroups(PrivateMsgDTO MsgDTO)
        {
            var list = AvailableGroups;
            string msg = $"共有群组{list.Count}个";
            foreach (var l in list)
            {
                msg += '\r' + l.ToString();
            }

            Common.SendMsgToDeveloper(msg);
        }

        [EnterCommand(Command = "清空报时", SourceType = MsgType.Group, AuthorityLevel = AuthorityLevel.群主)]
        public void ClearAlert(GroupMsgDTO MsgDTO)
        {
            if (string.IsNullOrEmpty(MsgDTO.msg))
            {
                return;
            }

            long num;
            if (!long.TryParse(MsgDTO.msg, out num))
            {
                return;
            }

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

        [EnterCommand(Command = "所有报时数目", SourceType = MsgType.Private, IsDeveloperOnly = true)]
        public void TotalAlertCount(PrivateMsgDTO MsgDTO)
        {
            Common.SendMsgToDeveloper(AllAlertInfos.Count().ToString());
        }
    }
}