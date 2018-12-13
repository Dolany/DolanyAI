using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Dolany.Ai.Core.Ai.Record
{
    using Dolany.Ai.Core.Base;
    using Dolany.Ai.Core.Cache;
    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Db;
    using static Dolany.Ai.Core.Common.Utility;
    using static Dolany.Ai.Core.API.CodeApi;

    [AI(
        Name = nameof(HourAlertAI),
        Description = "AI for Hour Alert.",
        IsAvailable = true,
        PriorityLevel = 10)]
    public class HourAlertAI : AIBase
    {
        private static List<long> AvailableGroups
        {
            get
            {
                using (var db = new AIDatabase())
                {
                    var selfNum = SelfQQNum;
                    var query = db.AlertRegistedGroup.Where(a => a.Available.ToLower() == "true" &&
                                                                 a.AINum == selfNum);
                    return query.IsNullOrEmpty() ? null : query.Select(q => q.GroupNum).ToList();
                }
            }
        }

        public HourAlertAI()
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
            JobScheduler.Instance.Add(ts.TotalMilliseconds, TimeUp);
        }

        private void TimeUp(object sender, System.Timers.ElapsedEventArgs e)
        {
            var timer = sender as JobTimer;

            HourAlert(DateTime.Now.Hour);
            Debug.Assert(timer != null, nameof(timer) + " != null");
            timer.Interval = GetNextHourSpan().TotalMilliseconds;
        }

        private static TimeSpan GetNextHourSpan()
        {
            var nextHour = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")).AddHours(1).AddSeconds(3);
            return nextHour - DateTime.Now;
        }

        private static void HourAlert(int curHour)
        {
            var availableList = AvailableGroups;
            if (availableList.IsNullOrEmpty())
            {
                return;
            }

            using (var db = new AIDatabase())
            {
                foreach (var groupNum in availableList)
                {
                    var isActiveOff = db.ActiveOffGroups.Any(p => p.GroupNum == groupNum);
                    if (isActiveOff)
                    {
                        continue;
                    }

                    var randGirl = GetRanAlertContent(curHour);
                    SendAlertMsg(randGirl, groupNum);
                }
            }
        }

        private static void SendAlertMsg(KanColeGirlVoice randGirl, long groupNum)
        {
            MsgSender.Instance.PushMsg(
                new MsgCommand
                    {
                        Command = AiCommand.SendGroup,
                        Id = Guid.NewGuid().ToString(),
                        Msg = Code_Voice(randGirl.VoiceUrl),
                        Time = DateTime.Now,
                        ToGroup = groupNum
                });
            MsgSender.Instance.PushMsg(
                new MsgCommand
                    {
                        Command = AiCommand.SendGroup,
                        Id = Guid.NewGuid().ToString(),
                        Msg = randGirl.Content,
                        Time = DateTime.Now,
                        ToGroup = groupNum
                    });
        }

        [EnterCommand(
            Command = "报时开启",
            AuthorityLevel = AuthorityLevel.管理员,
            Description = "设置报时功能开启",
            Syntax = "",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailabe = false)]
        public void AlertEnable(MsgInformationEx MsgDTO, object[] param)
        {
            AvailableStateChange(MsgDTO.FromGroup, true);
            MsgSender.Instance.PushMsg(MsgDTO, "报时功能已开启！");
        }

        [EnterCommand(
            Command = "报时关闭",
            AuthorityLevel = AuthorityLevel.管理员,
            Description = "设置报时功能关闭",
            Syntax = "",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailabe = false)]
        public void AlertDisenable(MsgInformationEx MsgDTO, object[] param)
        {
            AvailableStateChange(MsgDTO.FromGroup, false);
            MsgSender.Instance.PushMsg(MsgDTO, "报时功能已关闭！");
        }

        private static void AvailableStateChange(long groupNumber, bool state)
        {
            using (var db = new AIDatabase())
            {
                var query = db.AlertRegistedGroup.Where(a => a.GroupNum == groupNumber);
                if (query.IsNullOrEmpty())
                {
                    db.AlertRegistedGroup.Add(new AlertRegistedGroup
                    {
                        Id = Guid.NewGuid().ToString(),
                        GroupNum = groupNumber,
                        Available = state.ToString(),
                        AINum = SelfQQNum
                    });
                }
                else
                {
                    var arg = query.FirstOrDefault();
                    Debug.Assert(arg != null, nameof(arg) + " != null");
                    arg.Available = state.ToString();
                }
                db.SaveChanges();
            }
        }

        private static KanColeGirlVoice GetRanAlertContent(int aimHour)
        {
            using (var db = new AIDatabase())
            {
                var tag = HourToTag(aimHour);
                var query = db.KanColeGirlVoice.Where(a => a.Tag == tag)
                                               .OrderBy(a => a.Id);

                var randIdx = RandInt(query.Count());

                return query.Skip(randIdx)
                            .First()
                            .Clone();
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

        [EnterCommand(
            Command = "所有报时开启群组",
            Description = "获取所有报时开启群组的列表",
            Syntax = "",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailabe = true)]
        public void AllAvailabeGroups(MsgInformationEx MsgDTO, object[] param)
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

            SendMsgToDeveloper(msg);
        }
    }
}
