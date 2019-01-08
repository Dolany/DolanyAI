namespace Dolany.Ai.Core.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Timers;

    using Dolany.Ai.Core.API;
    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Db;

    using JetBrains.Annotations;

    public static class GroupMemberInfoCacher
    {
        private static object lock_obj { get; } = new object();

        private static Queue<long> WaitQueue { get; set; }

        private static int GroupEmptyRefreshRate => int.Parse(Utility.GetConfig("GroupEmptyRefreshRate"));

        private static int GroupRefreshRate => int.Parse(Utility.GetConfig("GroupRefreshRate"));

        [CanBeNull]
        public static MemberRoleCache GetMemberInfo(MsgInformationEx MsgDTO)
        {
            using (var db = new AIDatabase())
            {
                var query = db.MemberRoleCache.Where(
                    ic => ic.QQNum == MsgDTO.FromQQ && ic.GroupNum == MsgDTO.FromGroup);
                if (query.IsNullOrEmpty())
                {
                    Enqueue(MsgDTO.FromGroup);
                    return new MemberRoleCache { GroupNum = MsgDTO.FromGroup, QQNum = MsgDTO.FromQQ };
                }

                var Cache = query.FirstOrDefault();
                if (Cache != null && Cache.Datatime.AddDays(7) >= DateTime.Now)
                {
                    return Cache.Clone();
                }

                Enqueue(MsgDTO.FromGroup);
                return new MemberRoleCache { GroupNum = MsgDTO.FromGroup, QQNum = MsgDTO.FromQQ };
            }
        }

        private static void Enqueue(long GroupNum)
        {
            lock (lock_obj)
            {
                if (WaitQueue == null)
                {
                    WaitQueue = new Queue<long>();
                    JobScheduler.Instance.Add(JobTimer.HourlyInterval * GroupEmptyRefreshRate, TimeUp);
                }

                if (!WaitQueue.Contains(GroupNum))
                {
                    WaitQueue.Enqueue(GroupNum);
                }
            }
        }

        private static void TimeUp(object sender, ElapsedEventArgs e)
        {
            var timer = sender as JobTimer;
            lock (lock_obj)
            {
                if (WaitQueue.Count == 0)
                {
                    if (timer != null)
                    {
                        timer.Interval = JobTimer.HourlyInterval * GroupEmptyRefreshRate;
                    }
                }
                else
                {
                    var groupNum = WaitQueue.Dequeue();
                    RefreshGroupInfo(groupNum);

                    if (timer != null)
                    {
                        timer.Interval = JobTimer.HourlyInterval * GroupRefreshRate;
                    }
                }
            }
        }

        private static void RefreshGroupInfo(long GroupNum)
        {
            using (var db = new AIDatabase())
            {
                var infos = APIEx.GetMemberInfos(GroupNum);
                if (infos?.Mems == null)
                {
                    RuntimeLogger.Log($"Cannot get Group Member Infos:{GroupNum}");
                    return;
                }

                foreach (var info in infos.Mems)
                {
                    var query = db.MemberRoleCache.Where(p => p.GroupNum == GroupNum && p.QQNum == info.Uin);
                    if (!query.IsNullOrEmpty())
                    {
                        var cache = query.First();
                        cache.Role = info.Role;
                        cache.Datatime = DateTime.Now;
                        cache.Nickname = info.Nick;
                    }
                    else
                    {
                        db.MemberRoleCache.Add(
                            new MemberRoleCache
                                {
                                    GroupNum = GroupNum, Nickname = info.Nick, QQNum = info.Uin, Role = info.Role
                                });
                    }
                }

                db.SaveChanges();
            }
        }
    }
}
