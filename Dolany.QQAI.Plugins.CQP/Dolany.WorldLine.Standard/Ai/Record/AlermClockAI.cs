﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.WorldLine.Standard.Ai.Record
{
    public class AlermClockAI : AIBase
    {
        public override string AIName { get; set; } = "闹钟";

        public override string Description { get; set; } = "AI for Alerm Clock.";

        public override AIPriority PriorityLevel { get;} = AIPriority.Normal;

        public override bool NeedManualOpeon { get; } = true;

        public override bool Enable { get; } = false;

        private List<string> ClockIdList { get; } = new List<string>();

        public SchedulerSvc SchedulerSvc { get; set; }

        public override void Initialization()
        {
            ReloadAllClocks();
        }

        private void ReloadAllClocks()
        {
            foreach (var clockId in ClockIdList)
            {
                SchedulerSvc.Remove(clockId);
            }
            ClockIdList.Clear();

            LoadAlerms(StartClock);
        }

        [EnterCommand(ID = "AlermClockAI_SetClock",
            Command = "设定闹钟 设置闹钟",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "设定在指定时间的闹钟，我会到时候艾特你并显示提醒内容",
            Syntax = "[目标时间] [提醒内容]",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "HourMinute Word",
            IsPrivateAvailable = false)]
        public bool SetClock(MsgInformationEx MsgDTO, object[] param)
        {
            if (!(param[0] is HourMinuteModel time))
            {
                return false;
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
                BindAi = MsgDTO.BindAi
            };

            InsertClock(entity, MsgDTO);
            return true;
        }

        private void InsertClock(AlermClock entity, MsgInformationEx MsgDTO)
        {
            InsertClock(entity, MsgDTO, StartClock);

            MsgSender.PushMsg(MsgDTO, "闹钟设定成功！");
        }

        private void StartClock(AlermClock entity)
        {
            var interval = GetNextInterval(entity.AimHourt, entity.AimMinute);
            var clockId = SchedulerSvc.Add(interval, TimeUp, entity);
            ClockIdList.Add(clockId);
        }

        private void TimeUp(object sender, ElapsedEventArgs e)
        {
            var timer = sender as SchedulerTimer;
            if (!(timer?.Data is AlermClock entity))
            {
                return;
            }

            var setting = GroupSettingSvc[entity.GroupNumber];
            if (setting.ExpiryTime.HasValue && setting.ExpiryTime.Value > DateTime.Now && setting.IsPowerOn)
            {
                MsgSender.PushMsg(
                    new MsgCommand
                    {
                        Command = CommandType.SendGroup,
                        Msg = $@"{CodeApi.Code_At(entity.Creator)} {entity.Content}",
                        ToGroup = entity.GroupNumber,
                        BindAi = entity.BindAi
                    });
            }

            timer.Interval = GetNextInterval(entity.AimHourt, entity.AimMinute);
        }

        [EnterCommand(ID = "AlermClockAI_QueryClock",
            Command = "我的闹钟",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查询你当前设置的闹钟",
            Syntax = "",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool QueryClock(MsgInformationEx MsgDTO, object[] param)
        {
            var Msg = QueryClock(MsgDTO);
            MsgSender.PushMsg(MsgDTO, Msg);
            return true;
        }

        [EnterCommand(ID = "AlermClockAI_DeleteClock",
            Command = "删除闹钟",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "删除指定时间的已经设置好的闹钟",
            Syntax = "[目标时间]",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "HourMinute",
            IsPrivateAvailable = false)]
        public bool DeleteClock(MsgInformationEx MsgDTO, object[] param)
        {
            if (!(param[0] is HourMinuteModel time))
            {
                return false;
            }

            var Msg = DeleteClock(time, MsgDTO);
            MsgSender.PushMsg(MsgDTO, Msg);

            ReloadAllClocks();
            return true;
        }

        [EnterCommand(ID = "AlermClockAI_ClearAllClock",
            Command = "清空闹钟",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "清空设置过的所有闹钟",
            Syntax = "",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool ClearAllClock(MsgInformationEx MsgDTO, object[] param)
        {
            var Msg = ClearAllClock(MsgDTO);
            MsgSender.PushMsg(MsgDTO, Msg);

            ReloadAllClocks();
            return true;
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

        private void LoadAlerms(Action<AlermClock> StartClock)
        {
            var Groups = Global.AllGroupsDic.Keys.ToArray();
            var clocks = MongoService<AlermClock>.Get(p => Groups.Contains(p.GroupNumber));
            foreach (var clock in from clock in clocks let isActiveOff = !GroupSettingSvc[clock.GroupNumber].IsPowerOn where !isActiveOff select clock)
            {
                StartClock(clock.Clone());
            }
        }

        private static void InsertClock(AlermClock entity, MsgInformation MsgDTO, Action<AlermClock> StartClock)
        {
            var query = MongoService<AlermClock>.Get(q => q.GroupNumber == MsgDTO.FromGroup &&
                                                          q.Creator == MsgDTO.FromQQ &&
                                                          q.AimHourt == entity.AimHourt &&
                                                          q.AimMinute == entity.AimMinute);
            if (!query.IsNullOrEmpty())
            {
                var clock = query.FirstOrDefault();
                Debug.Assert(clock != null, nameof(clock) + " != null");
                clock.Content = entity.Content;

                MongoService<AlermClock>.Update(clock);
            }
            else
            {
                MongoService<AlermClock>.Insert(entity);
                StartClock(entity.Clone());
            }
        }

        private static string QueryClock(MsgInformationEx MsgDTO)
        {
            var allClocks = MongoService<AlermClock>.Get(q => q.GroupNumber == MsgDTO.FromGroup &&
                                                              q.Creator == MsgDTO.FromQQ);
            if (allClocks.IsNullOrEmpty())
            {
                return $@"{CodeApi.Code_At(MsgDTO.FromQQ)} 你还没有设定闹钟呢！";
            }

            var Msg = $@"{CodeApi.Code_At(MsgDTO.FromQQ)} 你当前共设定了{allClocks.Count()}个闹钟";
            var builder = new StringBuilder();
            builder.Append(Msg);
            foreach (var clock in allClocks)
            {
                builder.Append('\r' + $@"{clock.AimHourt:00}:{clock.AimMinute:00} {clock.Content}");
            }
            Msg = builder.ToString();

            return Msg;
        }

        private static string DeleteClock(HourMinuteModel time, MsgInformation MsgDTO)
        {
            var query = MongoService<AlermClock>.Get(q => q.GroupNumber == MsgDTO.FromGroup &&
                                                          q.Creator == MsgDTO.FromQQ &&
                                                          q.AimHourt == time.Hour &&
                                                          q.AimMinute == time.Minute);
            if (query.IsNullOrEmpty())
            {
                return "八嘎！你还没有在这个时间点设置过闹钟呢！";
            }

            MongoService<AlermClock>.DeleteMany(query);

            return "删除闹钟成功！";
        }

        private static string ClearAllClock(MsgInformation MsgDTO)
        {
            var query = MongoService<AlermClock>.Get(q => q.GroupNumber == MsgDTO.FromGroup &&
                                                          q.Creator == MsgDTO.FromQQ);
            if (query.IsNullOrEmpty())
            {
                return "八嘎！你还没有设置过闹钟呢！";
            }

            MongoService<AlermClock>.DeleteMany(query);

            return "清空闹钟成功！";
        }
    }
}
