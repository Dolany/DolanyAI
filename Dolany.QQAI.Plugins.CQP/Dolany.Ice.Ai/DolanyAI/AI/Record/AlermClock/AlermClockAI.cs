using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Dolany.Ice.Ai.DolanyAI.Db;
using System.Timers;
using Dolany.Ice.Ai.MahuaApis;

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
        public static List<string> ClockIdList => new List<string>();

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

            using (var db = new AIDatabase())
            {
                var clocks = db.AlermClock;
                foreach (var clock in clocks)
                {
                    StartClock(clock.Clone());
                }
            }
        }

        [EnterCommand(
            Command = "设定闹钟",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "设定在指定时间的闹钟，我会到时候艾特你并显示提醒内容",
            Syntax = "[目标时间] [提醒内容]",
            Tag = "闹钟与报时功能",
            SyntaxChecker = nameof(SetClock)
            )]
        [EnterCommand(
            Command = "设置闹钟",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "功能同设定闹钟",
            Syntax = "[目标时间] [提醒内容]",
            Tag = "闹钟与报时功能",
            SyntaxChecker = nameof(SetClock)
            )]
        public void SetClock(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var time = param[0] as (int hour, int minute)?;

            using (new AIDatabase())
            {
                Debug.Assert(time != null, nameof(time) + " != null");
                var entity = new AlermClock
                {
                    Id = Guid.NewGuid().ToString(),
                    AimHourt = time.Value.hour,
                    AimMinute = time.Value.minute,
                    Content = param[1] as string,
                    Creator = MsgDTO.FromQQ,
                    GroupNumber = MsgDTO.FromGroup,
                    CreateTime = DateTime.Now
                };

                InsertClock(entity, MsgDTO);
            }
        }

        private void InsertClock(AlermClock entity, ReceivedMsgDTO MsgDTO)
        {
            using (var db = new AIDatabase())
            {
                var query = db.AlermClock.Where(q => q.GroupNumber == MsgDTO.FromGroup
                                                    && q.Creator == MsgDTO.FromQQ
                                                    && q.AimHourt == entity.AimHourt
                                                    && q.AimMinute == entity.AimMinute);
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

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = "闹钟设定成功！"
            });
        }

        private void StartClock(AlermClock entity)
        {
            JobScheduler.Instance.Add(GetNextInterval(entity.AimHourt, entity.AimMinute), TimeUp, entity);
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
                Msg = $@"{CodeApi.Code_At(entity.Creator)} {entity.Content}"
            });

            timer.Interval = GetNextInterval(entity.AimHourt, entity.AimMinute);
        }

        [EnterCommand(
            Command = "我的闹钟",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查询你当前设置的闹钟",
            Syntax = "",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "Empty"
            )]
        public void QueryClock(ReceivedMsgDTO MsgDTO, object[] param)
        {
            using (var db = new AIDatabase())
            {
                var allClocks = db.AlermClock.Where(q => q.GroupNumber == MsgDTO.FromGroup
                                                                && q.Creator == MsgDTO.FromQQ);
                if (allClocks.IsNullOrEmpty())
                {
                    MsgSender.Instance.PushMsg(new SendMsgDTO
                    {
                        Aim = MsgDTO.FromGroup,
                        Type = MsgType.Group,
                        Msg = $@"{CodeApi.Code_At(MsgDTO.FromQQ)} 你还没有设定闹钟呢！"
                    });
                    return;
                }

                var Msg = $@"{CodeApi.Code_At(MsgDTO.FromQQ)} 你当前共设定了{allClocks.Count()}个闹钟";
                var builder = new StringBuilder();
                builder.Append(Msg);
                foreach (var clock in allClocks)
                {
                    builder.Append('\r' + $@"{clock.AimHourt:00}:{clock.AimMinute:00} {clock.Content}");
                }
                Msg = builder.ToString();

                MsgSender.Instance.PushMsg(new SendMsgDTO
                {
                    Aim = MsgDTO.FromGroup,
                    Type = MsgType.Group,
                    Msg = Msg
                });
            }
        }

        [EnterCommand(
            Command = "删除闹钟",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "删除指定时间的已经设置好的闹钟",
            Syntax = "[目标时间]",
            Tag = "闹钟与报时功能",
            SyntaxChecker = nameof(DeleteClock)
            )]
        public void DeleteClock(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var time = param[0] as (int hour, int minute)?;

            using (var db = new AIDatabase())
            {
                var query = db.AlermClock.Where(q => q.GroupNumber == MsgDTO.FromGroup
                                                     && q.Creator == MsgDTO.FromQQ
                                                     && q.AimHourt == time.Value.hour
                                                     && q.AimMinute == time.Value.minute
                                              );
                if (query.IsNullOrEmpty())
                {
                    MsgSender.Instance.PushMsg(new SendMsgDTO
                    {
                        Aim = MsgDTO.FromGroup,
                        Type = MsgType.Group,
                        Msg = "八嘎！你还没有在这个时间点设置过闹钟呢！"
                    });
                    return;
                }

                db.AlermClock.RemoveRange(query);

                db.SaveChanges();
                MsgSender.Instance.PushMsg(new SendMsgDTO
                {
                    Aim = MsgDTO.FromGroup,
                    Type = MsgType.Group,
                    Msg = "删除闹钟成功！"
                });
            }

            ReloadAllClocks();
        }

        [EnterCommand(
            Command = "清空闹钟",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "清空设置过的所有闹钟",
            Syntax = "",
            Tag = "闹钟与报时功能",
            SyntaxChecker = "Empty"
            )]
        public void ClearAllClock(ReceivedMsgDTO MsgDTO, object[] param)
        {
            using (var db = new AIDatabase())
            {
                var query = db.AlermClock.Where(q => q.GroupNumber == MsgDTO.FromGroup
                                                     && q.Creator == MsgDTO.FromQQ
                                              );
                if (query.IsNullOrEmpty())
                {
                    MsgSender.Instance.PushMsg(new SendMsgDTO
                    {
                        Aim = MsgDTO.FromGroup,
                        Type = MsgType.Group,
                        Msg = "八嘎！你还没有设置过闹钟呢！"
                    });

                    return;
                }

                db.AlermClock.RemoveRange(query);
                db.SaveChanges();

                MsgSender.Instance.PushMsg(new SendMsgDTO
                {
                    Aim = MsgDTO.FromGroup,
                    Type = MsgType.Group,
                    Msg = "清空闹钟成功！"
                });
            }

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
    }
}