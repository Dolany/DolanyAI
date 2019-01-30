namespace Dolany.Ai.Core.Ai.Record
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;

    using Base;

    using Cache;
    using Dolany.Ai.Common;
    using Database;
    using Dolany.Database.Ai;

    using Model;

    using static Common.Utility;

    using static API.CodeApi;

    [AI(
        Name = nameof(HourAlertAI),
        Description = "AI for Hour Alert.",
        Enable = true,
        PriorityLevel = 10)]
    public class HourAlertAI : AIBase
    {
        private static List<long> AvailableGroups
        {
            get
            {
                var selfNum = SelfQQNum;
                var query = MongoService<AlertRegistedGroup>.Get(a => a.Available.ToLower() == "true" &&
                                                                      a.AINum == selfNum);
                return query.IsNullOrEmpty() ? null : query.Select(q => q.GroupNum).ToList();
            }
        }

        public override void Initialization()
        {
            HourAlertFunc();
        }

        private void HourAlertFunc()
        {
            var ts = GetNextHourSpan();
            Scheduler.Instance.Add(ts.TotalMilliseconds, TimeUp);
        }

        private void TimeUp(object sender, System.Timers.ElapsedEventArgs e)
        {
            var timer = sender as SchedulerTimer;

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

            var groups = MongoService<ActiveOffGroups>.Get();
            foreach (var groupNum in availableList)
            {
                var isActiveOff = groups.Any(p => p.GroupNum == groupNum);
                if (isActiveOff)
                {
                    continue;
                }

                var randGirl = GetRanAlertContent(curHour);
                SendAlertMsg(randGirl, groupNum);

                Thread.Sleep(1000);
            }
        }

        private static void SendAlertMsg(KanColeGirlVoice randGirl, long groupNum)
        {
            MsgSender.Instance.PushMsg(
                new MsgCommand
                    {
                        Command = AiCommand.SendGroup,
                        Msg = Code_Voice(randGirl.VoiceUrl),
                        ToGroup = groupNum
                });
            MsgSender.Instance.PushMsg(
                new MsgCommand
                    {
                        Command = AiCommand.SendGroup,
                        Msg = randGirl.Content,
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
            IsPrivateAvailable = false)]
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
            IsPrivateAvailable = false)]
        public void AlertDisenable(MsgInformationEx MsgDTO, object[] param)
        {
            AvailableStateChange(MsgDTO.FromGroup, false);
            MsgSender.Instance.PushMsg(MsgDTO, "报时功能已关闭！");
        }

        private static void AvailableStateChange(long groupNumber, bool state)
        {
            var query = MongoService<AlertRegistedGroup>.Get(a => a.GroupNum == groupNumber);
            if (query.IsNullOrEmpty())
            {
                MongoService<AlertRegistedGroup>.Insert(new AlertRegistedGroup
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

                MongoService<AlertRegistedGroup>.Update(arg);
            }
        }

        private static KanColeGirlVoice GetRanAlertContent(int aimHour)
        {
            var tag = HourToTag(aimHour);
            var query = MongoService<KanColeGirlVoice>.Get(a => a.Tag == tag).OrderBy(a => a.Id).ToList();

            var randIdx = RandInt(query.Count());

            return query[randIdx];
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
    }
}
