﻿namespace Dolany.Ai.Core.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Timers;

    using API;

    using Common;

    using Dolany.Ai.Common;
    using Dolany.Database;
    using Dolany.Database.Ai;

    using JetBrains.Annotations;

    using Model;

    public static class GroupMemberInfoCacher
    {
        private static object lock_obj { get; } = new object();

        private static Queue<long> WaitQueue { get; set; }

        private static int GroupEmptyRefreshRate => int.Parse(CommonUtil.GetConfig("GroupEmptyRefreshRate"));

        private static int GroupRefreshRate => int.Parse(CommonUtil.GetConfig("GroupRefreshRate"));

        [CanBeNull]
        public static MemberRoleCache GetMemberInfo(MsgInformationEx MsgDTO)
        {
            var query = MongoService<MemberRoleCache>.Get(ic => ic.QQNum == MsgDTO.FromQQ && ic.GroupNum == MsgDTO.FromGroup);
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

        public static bool RefreshGroupInfo(long GroupNum)
        {
            var infos = APIEx.GetMemberInfos(GroupNum);
            if (infos?.Mems == null)
            {
                RuntimeLogger.Log($"Cannot get Group Member Infos:{GroupNum}");
                return false;
            }

            foreach (var info in infos.Mems)
            {
                var query = MongoService<MemberRoleCache>.Get(p => p.GroupNum == GroupNum && p.QQNum == info.Uin);
                if (!query.IsNullOrEmpty())
                {
                    var cache = query.First();
                    cache.Role = info.Role;
                    cache.Datatime = DateTime.Now;
                    cache.Nickname = info.Nick;

                    MongoService<MemberRoleCache>.Update(cache);
                }
                else
                {
                    MongoService<MemberRoleCache>.Insert(new MemberRoleCache
                    {
                        GroupNum = GroupNum, Nickname = info.Nick, QQNum = info.Uin, Role = info.Role
                    });
                }
            }

            return true;
        }
    }
}
