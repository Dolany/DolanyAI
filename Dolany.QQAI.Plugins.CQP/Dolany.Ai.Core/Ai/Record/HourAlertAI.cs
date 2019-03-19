using Dolany.Ai.Core.Common;

namespace Dolany.Ai.Core.Ai.Record
{
    using System;
    using System.Linq;
    using System.Threading;

    using Base;

    using Cache;
    using Dolany.Ai.Common;
    using Database;
    using Dolany.Database.Ai;
    using static API.CodeApi;

    [AI(
        Name = "报时",
        Description = "AI for Hour Alert.",
        Enable = true,
        PriorityLevel = 10,
        NeedManulOpen = true)]
    public class HourAlertAI : AIBase
    {
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
            HourAlert(DateTime.Now.Hour);
            if (sender is SchedulerTimer timer)
            {
                timer.Interval = GetNextHourSpan().TotalMilliseconds;
            }
        }

        private static TimeSpan GetNextHourSpan()
        {
            var nextHour = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:00")).AddHours(1).AddSeconds(3);
            return nextHour - DateTime.Now;
        }

        private static void HourAlert(int curHour)
        {
            var availableList = GroupSettingMgr.Instance.SettingDic.Where(p => p.Value.HasFunction("报时")).Select(p => p.Key);
            if (availableList.IsNullOrEmpty())
            {
                return;
            }

            foreach (var groupNum in availableList)
            {
                var isActiveOff = !GroupSettingMgr.Instance[groupNum].IsPowerOn;
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

        private static KanColeGirlVoice GetRanAlertContent(int aimHour)
        {
            var tag = HourToTag(aimHour);
            var query = MongoService<KanColeGirlVoice>.Get(a => a.Tag == tag).OrderBy(a => a.Id).ToList();

            var randIdx = CommonUtil.RandInt(query.Count);

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
