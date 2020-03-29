using System;
using System.Linq;
using System.Threading;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.WorldLine.Standard.Ai.Record
{
    public class HourAlertAI : AIBase
    {
        public override string AIName { get; set; } = "报时";

        public override string Description { get; set; } = "AI for Hour Alert.";

        public override bool Enable { get; } = false;

        public SchedulerSvc SchedulerSvc { get; set; }

        public override void Initialization()
        {
            HourAlertFunc();
        }

        private void HourAlertFunc()
        {
            var ts = GetNextHourSpan();
            SchedulerSvc.Add(ts.TotalMilliseconds, TimeUp);
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

        private void HourAlert(int curHour)
        {
            var availableList = GroupSettingSvc.SettingDic.Where(p => p.Value.ExpiryTime.HasValue &&
                                                                      p.Value.ExpiryTime.Value > DateTime.Now).Select(p => p.Key).ToList();
            if (availableList.IsNullOrEmpty())
            {
                return;
            }

            foreach (var groupNum in availableList)
            {
                var groupSetting = GroupSettingSvc[groupNum];
                var isActiveOff = !groupSetting.IsPowerOn;
                if (isActiveOff)
                {
                    continue;
                }

                var randGirl = GetRanAlertContent(curHour);
                SendAlertMsg(randGirl, groupNum, groupSetting.BindAi);

                Thread.Sleep(1000);
            }
        }

        private static void SendAlertMsg(KanColeGirlVoice randGirl, long groupNum, string BindAi)
        {
            MsgSender.PushMsg(new MsgCommand {Command = CommandType.SendGroup, Msg = CodeApi.Code_Voice(randGirl.VoiceUrl), ToGroup = groupNum, BindAi = BindAi});
            MsgSender.PushMsg(new MsgCommand {Command = CommandType.SendGroup, Msg = randGirl.Content, ToGroup = groupNum, BindAi = BindAi});
        }

        private static KanColeGirlVoice GetRanAlertContent(int aimHour)
        {
            var tag = HourToTag(aimHour);
            var query = MongoService<KanColeGirlVoice>.Get(a => a.Tag == tag).OrderBy(a => a.Id).ToList();
            return query.RandElement();
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
