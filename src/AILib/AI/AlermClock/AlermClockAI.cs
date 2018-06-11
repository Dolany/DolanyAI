﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib.Entities;
using Flexlive.CQP.Framework;

namespace AILib
{
    [AI(Name = "AlermClockAI", Description = "AI for Alerm Clock.", IsAvailable = true)]
    public class AlermClockAI : AIBase
    {
        private List<AlermClockEntity> ClockList = new List<AlermClockEntity>();

        public AlermClockAI(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {

        }

        public override void Work()
        {
            LoadAllClocks();
        }

        private void LoadAllClocks()
        {
            var clocks = DbMgr.Query<AlermClockEntity>();
            if(clocks == null || clocks.Count() == 0)
            {
                return;
            }

            foreach(var clock in clocks)
            {
                StartClock(clock);
            }
        }

        [EnterCommand(Command = "设置闹钟", SourceType = MsgType.Group)]
        public void SetClock(GroupMsgDTO MsgDTO)
        {
            string[] strs = MsgDTO.msg.Split(new char[] { ' ' });
            if(strs == null || strs.Length < 2)
            {
                return;
            }

            (int hour, int minute)? time = GenTimeFromStr(strs[0]);
            if(time == null)
            {
                return;
            }

            if(string.IsNullOrEmpty(strs[1]))
            {
                return;
            }

            AlermClockEntity entity = new AlermClockEntity()
            {
                Id = Guid.NewGuid().ToString(),
                AimHourt = time.Value.hour,
                AimMinute = time.Value.minute,
                Content = strs[1],
                Creator = MsgDTO.fromQQ,
                GroupNumber = MsgDTO.fromGroup,
                CreateTime = DateTime.Now
            };

            InsertClock(entity, MsgDTO);
        }

        private void InsertClock(AlermClockEntity entity, GroupMsgDTO MsgDTO)
        {
            try
            {
                var query = DbMgr.Query<AlermClockEntity>(q => q.GroupNumber == MsgDTO.fromGroup
                                                    && q.Creator == MsgDTO.fromQQ
                                                    && q.AimHourt == entity.AimHourt
                                                    && q.AimMinute == entity.AimMinute);
                if(query != null && query.Count() > 0)
                {
                    MsgSender.Instance.PushMsg(new SendMsgDTO()
                    {
                        Aim = MsgDTO.fromGroup,
                        Type = MsgType.Group,
                        Msg = $@"{CQ.CQCode_At(MsgDTO.fromQQ)} 八嘎！你已经在这个时间点设置过闹钟啦！"
                    });
                    return;
                }

                DbMgr.Insert(entity);
                StartClock(entity);

                MsgSender.Instance.PushMsg(new SendMsgDTO()
                {
                    Aim = MsgDTO.fromGroup,
                    Type = MsgType.Group,
                    Msg = "闹钟设定成功！"
                });
            }
            catch (Exception ex)
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO()
                {
                    Aim = MsgDTO.fromGroup,
                    Type = MsgType.Group,
                    Msg = "闹钟设定失败！"
                });
                Common.SendMsgToDeveloper(ex);
            }
        }

        private void StartClock(AlermClockEntity entity)
        {
            // TODO
        }

        [EnterCommand(Command = "我的闹钟", SourceType = MsgType.Group)]
        public void QueryClock(GroupMsgDTO MsgDTO)
        {
            var allClocks = DbMgr.Query<AlermClockEntity>(q => q.GroupNumber == MsgDTO.fromGroup 
                                                                && q.Creator == MsgDTO.fromQQ);
            if(allClocks == null || allClocks.Count() == 0)
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO()
                {
                    Aim = MsgDTO.fromGroup,
                    Type = MsgType.Group,
                    Msg = $@"{CQ.CQCode_At(MsgDTO.fromQQ)} 你当前没有设定闹钟！"
                });
                return;
            }

            string Msg = $@"{CQ.CQCode_At(MsgDTO.fromQQ)} 你当前共设定了{allClocks.Count()}个闹钟";
            foreach(var clock in allClocks)
            {
                Msg += '\r' + $@"{clock.AimHourt.ToString("00")}:{clock.AimMinute.ToString("00")} {clock.Content}";
            }

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = Msg
            });
        }

        [EnterCommand(Command = "删除闹钟", SourceType = MsgType.Group)]
        public void DeleteClock(GroupMsgDTO MsgDTO)
        {
            (int hour, int minute)? time = GenTimeFromStr(MsgDTO.msg);
            if (time == null)
            {
                return;
            }

            if(DbMgr.Delete<AlermClockEntity>(q => q.GroupNumber == MsgDTO.fromGroup
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
                    Msg = "你还没有在这个时间点设置过闹钟呢！"
                });
            }
        }

        private (int hour, int minute)? GenTimeFromStr(string timeStr)
        {
            string[] strs = timeStr.Split(new char[] { ':', '：' });
            if(strs == null || strs.Length != 2)
            {
                return null;
            }

            int hour;
            if(!int.TryParse(strs[0], out hour))
            {
                return null;
            }

            int minute;
            if (!int.TryParse(strs[1], out minute))
            {
                return null;
            }

            return (hour, minute);
        }
    }
}
