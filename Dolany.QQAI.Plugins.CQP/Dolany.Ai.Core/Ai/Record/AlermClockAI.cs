using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace Dolany.Ai.Core.Ai.Record
{
    using Dolany.Ai.Core.Base;
    using Dolany.Ai.Core.Cache;
    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Model;
    using Dolany.Database.Ai;

    using static Dolany.Ai.Core.API.CodeApi;
    using static Dolany.Ai.Core.Common.Utility;

    [AI(
        Name = nameof(AlermClockAI),
        Description = "AI for Alerm Clock.",
        IsAvailable = true,
        PriorityLevel = 10)]
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

            LoadAlerms(StartClock);
        }

        [EnterCommand(
            Command = "设定闹钟 设置闹钟",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "设定在指定时间的闹钟，我会到时候艾特你并显示提醒内容",
            Syntax = "[目标时间] [提醒内容]",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "HourMinute Word",
            IsPrivateAvailable = false)]
        public void SetClock(MsgInformationEx MsgDTO, object[] param)
        {
            if (!(param[0] is HourMinuteModel time))
            {
                return;
            }

            var entity = new AlermClock
            {
                Id = Guid.NewGuid().ToString(),
                AimHourt = time.Hour,
                AimMinute = time.Minute,
                Content = param[1] as string,
                Creator = MsgDTO.FromQQ,
                GroupNumber = MsgDTO.FromGroup,
                CreateTime = DateTime.Now,
                AINum = SelfQQNum
            };

            InsertClock(entity, MsgDTO);
        }

        private void InsertClock(AlermClock entity, MsgInformationEx MsgDTO)
        {
            InsertClock(entity, MsgDTO, StartClock);

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
            MsgSender.Instance.PushMsg(
                new MsgCommand
                    {
                        Id = Guid.NewGuid().ToString(),
                        Command = AiCommand.SendGroup,
                        Msg = $@"{Code_At(entity.Creator)} {entity.Content}",
                        Time = DateTime.Now,
                        ToGroup = entity.GroupNumber
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
            IsPrivateAvailable = false)]
        public void QueryClock(MsgInformationEx MsgDTO, object[] param)
        {
            var Msg = QueryClock(MsgDTO);
            MsgSender.Instance.PushMsg(MsgDTO, Msg);
        }

        [EnterCommand(
            Command = "删除闹钟",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "删除指定时间的已经设置好的闹钟",
            Syntax = "[目标时间]",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "HourMinute",
            IsPrivateAvailable = false)]
        public void DeleteClock(MsgInformationEx MsgDTO, object[] param)
        {
            if (!(param[0] is HourMinuteModel time))
            {
                return;
            }

            var Msg = DeleteClock(time, MsgDTO);
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
            IsPrivateAvailable = false)]
        public void ClearAllClock(MsgInformationEx MsgDTO, object[] param)
        {
            var Msg = ClearAllClock(MsgDTO);
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

        private static void LoadAlerms(Action<AlermClock> StartClock)
        {
            using (var db = new AIDatabase())
            {
                var selfNum = SelfQQNum;
                var clocks = db.AlermClock.Where(p => p.AINum == selfNum);
                foreach (var clock in clocks)
                {
                    var isActiveOff = db.ActiveOffGroups.Any(p => p.GroupNum == clock.GroupNumber);
                    if (isActiveOff)
                    {
                        continue;
                    }
                    StartClock(clock.Clone());
                }
            }
        }

        private static void InsertClock(AlermClock entity, MsgInformation MsgDTO, Action<AlermClock> StartClock)
        {
            using (var db = new AIDatabase())
            {
                var query = db.AlermClock.Where(q => q.GroupNumber == MsgDTO.FromGroup &&
                                                     q.Creator == MsgDTO.FromQQ &&
                                                     q.AimHourt == entity.AimHourt &&
                                                     q.AimMinute == entity.AimMinute);
                if (!query.IsNullOrEmpty())
                {
                    var clock = query.FirstOrDefault();
                    Debug.Assert(clock != null, nameof(clock) + " != null");
                    clock.Content = entity.Content;
                }
                else
                {
                    db.AlermClock.Add(entity);
                    StartClock(entity.Clone());
                }

                db.SaveChanges();
            }
        }

        private static string QueryClock(MsgInformationEx MsgDTO)
        {
            using (var db = new AIDatabase())
            {
                var allClocks = db.AlermClock.Where(q => q.GroupNumber == MsgDTO.FromGroup &&
                                                         q.Creator == MsgDTO.FromQQ);
                if (allClocks.IsNullOrEmpty())
                {
                    return $@"{Code_At(MsgDTO.FromQQ)} 你还没有设定闹钟呢！";
                }

                var Msg = $@"{Code_At(MsgDTO.FromQQ)} 你当前共设定了{allClocks.Count()}个闹钟";
                var builder = new StringBuilder();
                builder.Append(Msg);
                foreach (var clock in allClocks)
                {
                    builder.Append('\r' + $@"{clock.AimHourt:00}:{clock.AimMinute:00} {clock.Content}");
                }
                Msg = builder.ToString();

                return Msg;
            }
        }

        private static string DeleteClock(HourMinuteModel time, MsgInformationEx MsgDTO)
        {
            using (var db = new AIDatabase())
            {
                var query = db.AlermClock.Where(q => q.GroupNumber == MsgDTO.FromGroup &&
                                                     q.Creator == MsgDTO.FromQQ &&
                                                     q.AimHourt == time.Hour &&
                                                     q.AimMinute == time.Minute);
                if (query.IsNullOrEmpty())
                {
                    return "八嘎！你还没有在这个时间点设置过闹钟呢！";
                }

                db.AlermClock.RemoveRange(query);

                db.SaveChanges();
                return "删除闹钟成功！";
            }
        }

        private static string ClearAllClock(MsgInformationEx MsgDTO)
        {
            using (var db = new AIDatabase())
            {
                var query = db.AlermClock.Where(q => q.GroupNumber == MsgDTO.FromGroup &&
                                                     q.Creator == MsgDTO.FromQQ);
                if (query.IsNullOrEmpty())
                {
                    return "八嘎！你还没有设置过闹钟呢！";
                }

                db.AlermClock.RemoveRange(query);
                db.SaveChanges();

                return "清空闹钟成功！";
            }
        }

        public override void OnActiveStateChange(bool state, long GroupNum)
        {
            base.OnActiveStateChange(state, GroupNum);

            ReloadAllClocks();
        }
    }
}
