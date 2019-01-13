namespace Dolany.Ai.Core.Cache
{
    using System;

    using API;

    using Common;

    using Dolany.Database.Ai;
    using Dolany.Database.Redis;
    using Dolany.Database.Redis.Model;

    using JetBrains.Annotations;

    using Model;

    public static class GroupMemberInfoCacher
    {
        [CanBeNull]
        public static MemberRoleCache GetMemberInfo(MsgInformationEx MsgDTO)
        {
            var redisKey = $"GroupMemberInfo-{MsgDTO.FromGroup}-{MsgDTO.FromQQ}";
            var redisValue = Cache.Get<GroupMemberCacheModel>(redisKey);
            if (redisValue != null)
            {
                return new MemberRoleCache
                           {
                               GroupNum = redisValue.GroupNum,
                               QQNum = redisValue.QQNum,
                               Datatime = DateTime.Now,
                               Nickname = redisValue.NickName,
                               Role = redisValue.Role
                           };
            }

            RefreshGroupInfo(MsgDTO.FromGroup);

            return new MemberRoleCache { GroupNum = MsgDTO.FromGroup, QQNum = MsgDTO.FromQQ };
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
                var redisKey = $"GroupMemberInfo-{GroupNum}-{info.Uin}";
                var redisValue = Cache.Get<GroupMemberCacheModel>(redisKey);

                if (redisValue != null)
                {
                    redisValue.Role = info.Role;
                    redisValue.NickName = info.Nick;

                    Cache.Insert(redisKey, redisValue, DateTime.Now.AddDays(7));
                }
                else
                {
                    Cache.Insert(
                        redisKey,
                        new GroupMemberCacheModel
                            {
                                GroupNum = GroupNum, NickName = info.Nick, QQNum = info.Uin, Role = info.Role
                            },
                        DateTime.Now.AddDays(7));
                }
            }

            return true;
        }
    }
}
