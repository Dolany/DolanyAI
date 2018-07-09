using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib.Entities;
using Flexlive.CQP.Framework;
using System.Timers;
using Flexlive.CQP.Framework.Utils;
using System.ComponentModel.Composition;

namespace AILib
{
    public class TimerEx : Timer
    {
        public AlermClockEntity ClockEntity { get; set; }
    }

    [AI(
        Name = "AlermClockAI",
        Description = "AI for Alerm Clock.",
        IsAvailable = true,
        PriorityLevel = 10
        )]
    public class AlermClockAI : AIBase
    {
        private List<TimerEx> ClockList = new List<TimerEx>();

        public AlermClockAI()
            : base()
        {
            RuntimeLogger.Log("AlermClockAI constructed");
        }

        public override void Work()
        {
            ReloadAllClocks();
        }

        private void ReloadAllClocks()
        {
            RuntimeLogger.Log("AlermClockAI ReloadAllClocks");
            lock (ClockList)
            {
                foreach (var clock in ClockList)
                {
                    clock.Stop();
                    clock.Enabled = false;
                }
                ClockList.Clear();

                var clocks = DbMgr.Query<AlermClockEntity>();
                if (clocks.IsNullOrEmpty())
                {
                    return;
                }

                foreach (var clock in clocks)
                {
                    StartClock(clock);
                }
            }
            RuntimeLogger.Log("AlermClockAI ReloadAllClocks Completed");
        }

        [EnterCommand(
            Command = "设定闹钟",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.成员,
            Description = "设定在指定时间的闹钟，我会到时候艾特你并显示提醒内容",
            Syntax = "[目标时间] [提醒内容]",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "SetClock"
            )]
        [EnterCommand(
            Command = "设置闹钟",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.成员,
            Description = "功能同设定闹钟",
            Syntax = "[目标时间] [提醒内容]",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "SetClock"
            )]
        public void SetClock(GroupMsgDTO MsgDTO, object[] param)
        {
            RuntimeLogger.Log("AlermClockAI Tryto SetClock");

            (int hour, int minute)? time = param[0] as (int hour, int minute)?;

            AlermClockEntity entity = new AlermClockEntity()
            {
                Id = Guid.NewGuid().ToString(),
                AimHourt = time.Value.hour,
                AimMinute = time.Value.minute,
                Content = param[1] as string,
                Creator = MsgDTO.fromQQ,
                GroupNumber = MsgDTO.fromGroup,
                CreateTime = DateTime.Now
            };

            InsertClock(entity, MsgDTO);
            RuntimeLogger.Log("AlermClockAI SetClock Complete");
        }

        private void InsertClock(AlermClockEntity entity, GroupMsgDTO MsgDTO)
        {
            RuntimeLogger.Log("AlermClockAI InsertClock");
            var query = DbMgr.Query<AlermClockEntity>(q => q.GroupNumber == MsgDTO.fromGroup
                                                    && q.Creator == MsgDTO.fromQQ
                                                    && q.AimHourt == entity.AimHourt
                                                    && q.AimMinute == entity.AimMinute);
            if (!query.IsNullOrEmpty())
            {
                var clock = query.FirstOrDefault();
                clock.Content = entity.Content;
                DbMgr.Update(clock);
            }
            else
            {
                DbMgr.Insert(entity);
                StartClock(entity);
            }

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = "闹钟设定成功！"
            });
            RuntimeLogger.Log("AlermClockAI InsertClock Complete");
        }

        private void StartClock(AlermClockEntity entity)
        {
            TimerEx timer = new TimerEx();
            timer.ClockEntity = entity;
            timer.Enabled = true;
            timer.Interval = GetNextInterval(entity.AimHourt, entity.AimMinute);
            ClockList.Add(timer);
            timer.Elapsed += TimeUp;
            timer.AutoReset = false;

            timer.Start();
        }

        private void TimeUp(object sender, ElapsedEventArgs e)
        {
            RuntimeLogger.Log("AlermClockAI TimeUp");
            lock (ClockList)
            {
                TimerEx timer = sender as TimerEx;
                timer.Stop();

                MsgSender.Instance.PushMsg(new SendMsgDTO()
                {
                    Aim = timer.ClockEntity.GroupNumber,
                    Type = MsgType.Group,
                    Msg = $@"{CQ.CQCode_At(timer.ClockEntity.Creator)} {timer.ClockEntity.Content}"
                });

                timer.Interval = GetNextInterval(timer.ClockEntity.AimHourt, timer.ClockEntity.AimMinute);
                timer.Start();
            }
            RuntimeLogger.Log("AlermClockAI TimeUp Complete");
        }

        [EnterCommand(
            Command = "我的闹钟",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查询你当前设置的闹钟",
            Syntax = "",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "Empty"
            )]
        public void QueryClock(GroupMsgDTO MsgDTO, object[] param)
        {
            RuntimeLogger.Log("AlermClockAI Tryto QueryClock");
            var allClocks = DbMgr.Query<AlermClockEntity>(q => q.GroupNumber == MsgDTO.fromGroup
                                                                && q.Creator == MsgDTO.fromQQ);
            if (allClocks.IsNullOrEmpty())
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO()
                {
                    Aim = MsgDTO.fromGroup,
                    Type = MsgType.Group,
                    Msg = $@"{CQ.CQCode_At(MsgDTO.fromQQ)} 你还没有设定闹钟呢！"
                });
                return;
            }

            string Msg = $@"{CQ.CQCode_At(MsgDTO.fromQQ)} 你当前共设定了{allClocks.Count()}个闹钟";
            foreach (var clock in allClocks)
            {
                Msg += '\r' + $@"{clock.AimHourt.ToString("00")}:{clock.AimMinute.ToString("00")} {clock.Content}";
            }

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = Msg
            });
            RuntimeLogger.Log("AlermClockAI QueryClock Complete");
        }

        [EnterCommand(
            Command = "删除闹钟",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.成员,
            Description = "删除指定时间的已经设置好的闹钟",
            Syntax = "[目标时间]",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "DeleteClock"
            )]
        public void DeleteClock(GroupMsgDTO MsgDTO, object[] param)
        {
            RuntimeLogger.Log("AlermClockAI Tryto DeleteClock");
            (int hour, int minute)? time = param[0] as (int hour, int minute)?;

            if (DbMgr.Delete<AlermClockEntity>(q => q.GroupNumber == MsgDTO.fromGroup
                                                     && q.Creator == MsgDTO.fromQQ
                                                     && q.AimHourt == time.Value.hour
                                                     && q.AimMinute == time.Value.minute
                                              ) > 0)
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO()
                {
                    Aim = MsgDTO.fromGroup,
                    Type = MsgType.Group,
                    Msg = "删除闹钟成功！"
                });
            }
            else
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO()
                {
                    Aim = MsgDTO.fromGroup,
                    Type = MsgType.Group,
                    Msg = "八嘎！你还没有在这个时间点设置过闹钟呢！"
                });
            }

            ReloadAllClocks();
            RuntimeLogger.Log("AlermClockAI DeleteClock Complete");
        }

        [EnterCommand(
            Command = "清空闹钟",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.成员,
            Description = "清空设置过的所有闹钟",
            Syntax = "",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "Empty"
            )]
        public void ClearAllClock(GroupMsgDTO MsgDTO, object[] param)
        {
            RuntimeLogger.Log("AlermClockAI Tryto ClearAllClock");
            if (DbMgr.Delete<AlermClockEntity>(q => q.GroupNumber == MsgDTO.fromGroup
                                                     && q.Creator == MsgDTO.fromQQ
                                              ) > 0)
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO()
                {
                    Aim = MsgDTO.fromGroup,
                    Type = MsgType.Group,
                    Msg = "清空闹钟成功！"
                });
            }
            else
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO()
                {
                    Aim = MsgDTO.fromGroup,
                    Type = MsgType.Group,
                    Msg = "八嘎！你还没有设置过闹钟呢！"
                });
            }

            ReloadAllClocks();
            RuntimeLogger.Log("AlermClockAI ClearAllClock Complete");
        }

        private double GetNextInterval(int hour, int minute)
        {
            DateTime now = DateTime.Now;
            DateTime aimTime = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
            if (aimTime < now)
            {
                aimTime = aimTime.AddDays(1);
            }

            return (aimTime - now).TotalMilliseconds;
        }
    }
}