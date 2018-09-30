using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Dolany.Ice.Ai.DolanyAI.Utils;
using static Dolany.Ice.Ai.DolanyAI.Utils.Utility;
using static Dolany.Ice.Ai.DolanyAI.Utils.CodeApi;

namespace Dolany.Ice.Ai.DolanyAI.Db
{
    public static class AlermClockBLL
    {
        public static void LoadAlerms(Action<AlermClock> StartClock)
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

        public static void InsertClock(AlermClock entity, ReceivedMsgDTO MsgDTO, Action<AlermClock> StartClock)
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

        public static string QueryClock(ReceivedMsgDTO MsgDTO)
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

        public static string DeleteClock((int hour, int minute)? time, ReceivedMsgDTO MsgDTO)
        {
            using (var db = new AIDatabase())
            {
                var query = db.AlermClock.Where(q => q.GroupNumber == MsgDTO.FromGroup &&
                                                     q.Creator == MsgDTO.FromQQ &&
                                                     q.AimHourt == time.Value.hour &&
                                                     q.AimMinute == time.Value.minute);
                if (query.IsNullOrEmpty())
                {
                    return "八嘎！你还没有在这个时间点设置过闹钟呢！";
                }

                db.AlermClock.RemoveRange(query);

                db.SaveChanges();
                return "删除闹钟成功！";
            }
        }

        public static string ClearAllClock(ReceivedMsgDTO MsgDTO)
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
    }
}