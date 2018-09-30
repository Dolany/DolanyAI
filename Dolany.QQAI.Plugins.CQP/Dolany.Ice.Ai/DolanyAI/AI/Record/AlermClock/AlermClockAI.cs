using System;
using System.Collections.Generic;
using System.Diagnostics;
using Dolany.Ice.Ai.DolanyAI.Db;
using System.Timers;
using Dolany.Ice.Ai.DolanyAI.Utils;
using static Dolany.Ice.Ai.DolanyAI.Utils.Utility;
using static Dolany.Ice.Ai.DolanyAI.Utils.CodeApi;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(AlermClockAI),
        Description = "AI for Alerm Clock.",
        IsAvailable = true,
        PriorityLevel = 10
        )]
    public class AlermClockAI : AIBase
    {
        private static List<string> ClockIdList => new List<string>();

        public AlermClockAI()
        {
            RuntimeLogger.Log("AlermClockAI constructed");
        }

        public override void Work()
        {
            ReloadAllClocks();
        }

        private void ReloadAllClocks()
        {
            foreach (var clockId in ClockIdList)
            {
                JobScheduler.Instance.Remove(clockId);
            }
            ClockIdList.Clear();

            AlermClockBLL.LoadAlerms(StartClock);
        }

        [EnterCommand(
            Command = "设定闹钟 设置闹钟",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "设定在指定时间的闹钟，我会到时候艾特你并显示提醒内容",
            Syntax = "[目标时间] [提醒内容]",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "HourMinute Word",
            IsPrivateAvailabe = false
            )]
        public void SetClock(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var time = param[0] as (int hour, int minute)?;

            Debug.Assert(time != null, nameof(time) + " != null");
            var entity = new AlermClock
            {
                Id = Guid.NewGuid().ToString(),
                AimHourt = time.Value.hour,
                AimMinute = time.Value.minute,
                Content = param[1] as string,
                Creator = MsgDTO.FromQQ,
                GroupNumber = MsgDTO.FromGroup,
                CreateTime = DateTime.Now,
                AINum = SelfQQNum
            };

            InsertClock(entity, MsgDTO);
        }

        private void InsertClock(AlermClock entity, ReceivedMsgDTO MsgDTO)
        {
            AlermClockBLL.InsertClock(entity, MsgDTO, StartClock);

            MsgSender.Instance.PushMsg(MsgDTO, "闹钟设定成功！");
        }

        private void StartClock(AlermClock entity)
        {
            var interval = GetNextInterval(entity.AimHourt, entity.AimMinute);
            var clockId = JobScheduler.Instance.Add(interval, TimeUp, entity);
            ClockIdList.Add(clockId);
        }

        private void TimeUp(object sender, ElapsedEventArgs e)
        {
            var timer = sender as JobTimer;
            Debug.Assert(timer != null, nameof(timer) + " != null");
            var entity = timer.Data as AlermClock;

            Debug.Assert(entity != null, nameof(entity) + " != null");
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = entity.GroupNumber,
                Type = MsgType.Group,
                Msg = $@"{Code_At(entity.Creator)} {entity.Content}"
            });

            timer.Interval = GetNextInterval(entity.AimHourt, entity.AimMinute);
        }

        [EnterCommand(
            Command = "我的闹钟",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查询你当前设置的闹钟",
            Syntax = "",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailabe = false
            )]
        public void QueryClock(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var Msg = AlermClockBLL.QueryClock(MsgDTO);
            MsgSender.Instance.PushMsg(MsgDTO, Msg);
        }

        [EnterCommand(
            Command = "删除闹钟",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "删除指定时间的已经设置好的闹钟",
            Syntax = "[目标时间]",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "HourMinute",
            IsPrivateAvailabe = false
            )]
        public void DeleteClock(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var time = param[0] as (int hour, int minute)?;

            var Msg = AlermClockBLL.DeleteClock(time, MsgDTO);
            MsgSender.Instance.PushMsg(MsgDTO, Msg);

            ReloadAllClocks();
        }

        [EnterCommand(
            Command = "清空闹钟",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "清空设置过的所有闹钟",
            Syntax = "",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailabe = false
            )]
        public void ClearAllClock(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var Msg = AlermClockBLL.ClearAllClock(MsgDTO);
            MsgSender.Instance.PushMsg(MsgDTO, Msg);

            ReloadAllClocks();
        }

        private static double GetNextInterval(int hour, int minute)
        {
            var now = DateTime.Now;
            var aimTime = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
            if (aimTime < now)
            {
                aimTime = aimTime.AddDays(1);
            }

            return (aimTime - now).TotalMilliseconds;
        }

        public override void OnActiveStateChange(bool state, long GroupNum)
        {
            base.OnActiveStateChange(state, GroupNum);

            ReloadAllClocks();
        }
    }
}