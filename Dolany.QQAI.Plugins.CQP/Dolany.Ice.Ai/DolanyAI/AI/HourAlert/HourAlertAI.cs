using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolany.Ice.Ai.DolanyAI.Db;
using Dolany.Ice.Ai.MahuaApis;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(HourAlertAI),
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
                    try
                    {
                        var query = db.AlertRegistedGroup.Where(a => a.Available.ToLower() == "true");
                        if (query.IsNullOrEmpty())
                        {
                            return null;
                        }
                        return query.Select(q => q.GroupNum).ToList();
                    }
                    catch (Exception)
                    {
                        return null;
                    }
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
            var ts = GetNextHourSpan();
            timer = new System.Timers.Timer(ts.TotalMilliseconds)
            {
                AutoReset = false,
                Enabled = true
            };
            timer.Elapsed += new System.Timers.ElapsedEventHandler(TimeUp);

            timer.Start();
        }

        private void TimeUp(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Stop();

            HourAlert(DateTime.Now.Hour);
            timer.Interval = GetNextHourSpan().TotalMilliseconds;
            timer.Start();
        }

        private static TimeSpan GetNextHourSpan()
        {
            var nextHour = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")).AddHours(1).AddSeconds(3);
            return nextHour - DateTime.Now;
        }

        private void HourAlert(int curHour)
        {
            var availableList = AvailableGroups;
            if (availableList.IsNullOrEmpty())
            {
                return;
            }

            foreach (var groupNum in availableList)
            {
                var randGirl = GetRanAlertContent(groupNum, curHour);
                MsgSender.Instance.PushMsg(new SendMsgDTO()
                {
                    Aim = groupNum,
                    Type = MsgType.Group,
                    Msg = CodeApi.Code_Voice(randGirl.VoiceUrl)
                });
                MsgSender.Instance.PushMsg(new SendMsgDTO()
                {
                    Aim = groupNum,
                    Type = MsgType.Group,
                    Msg = randGirl.Content
                });
            }
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
            AvailableStateChange(MsgDTO.FromGroup, true);
            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = "报时功能已开启！"
            });
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
            AvailableStateChange(MsgDTO.FromGroup, false);
            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = "报时功能已关闭！"
            });
        }

        private static void AvailableStateChange(long groupNumber, bool state)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.AlertRegistedGroup.Where(a => a.GroupNum == groupNumber);
                if (query.IsNullOrEmpty())
                {
                    db.AlertRegistedGroup.Add(new AlertRegistedGroup
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

        [GroupEnterCommand(
            Command = "报时",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "设定指定小时的报时内容",
            Syntax = " [目标小时] [报时内容]",
            Tag = "闹钟与报时功能",
            SyntaxChecker = nameof(HourAlert)
            )]
        public void AlertSet(GroupMsgDTO MsgDTO, object[] param)
        {
            var info = param[0] as AlertContent;

            info.CreateTime = DateTime.Now;
            info.Creator = MsgDTO.FromQQ;
            info.FromGroup = MsgDTO.FromGroup;

            var Msg = SaveAlertContent(info) ? "报时内容保存成功！" : "报时内容保存失败！";
            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = Msg
            });
        }

        private static bool SaveAlertContent(AlertContent info)
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
                Utility.SendMsgToDeveloper(ex);
                return false;
            }
        }

        private static KanColeGirlVoice GetRanAlertContent(long fromGroup, int aimHour)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var tag = HourToTag(aimHour);
                var query = db.KanColeGirlVoice.Where(a => a.Tag == tag).OrderBy(a => a.Id);

                var random = new Random();
                var randIdx = random.Next(query.Count());

                return query.Skip(randIdx).First().Clone();
            }
        }

        private static string HourToTag(int aimHour)
        {
            var tag = aimHour.ToString();
            if (aimHour < 10)
            {
                tag = "0" + tag;
            }

            tag += "00";
            return tag;
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
            var msg = $"共有群组{list.Count}个";
            var builder = new StringBuilder();
            builder.Append(msg);
            foreach (var l in list)
            {
                builder.Append('\r' + l.ToString());
            }
            msg = builder.ToString();

            Utility.SendMsgToDeveloper(msg);
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
            var num = (long)param[0];

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
            Utility.SendMsgToDeveloper(AllAlertInfos.Count().ToString());
        }
    }
}