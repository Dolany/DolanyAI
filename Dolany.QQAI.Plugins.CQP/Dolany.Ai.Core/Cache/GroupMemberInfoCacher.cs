namespace Dolany.Ai.Core.Cache
{
    using System;

    using API;

    using Common;

    using Dolany.Database.Ai;

    using JetBrains.Annotations;

    using Model;

    public static class GroupMemberInfoCacher
    {
        [CanBeNull]
        public static MemberRoleCache GetMemberInfo(MsgInformationEx MsgDTO)
        {
            var cacheResponse = CacheWaiter.Instance.WaitForResponse<MemberRoleCache>(
                $"GroupMemberInfo-{MsgDTO.FromGroup}",
                MsgDTO.FromQQ.ToString());

            if (cacheResponse != null)
            {
                return cacheResponse;
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
                var model = new MemberRoleCache
                                {
                                    GroupNum = GroupNum,
                                    Datatime = DateTime.Now,
                                    Nickname = info.Nick,
                                    QQNum = info.Uin,
                                    Role = info.Role
                                };
                CacheWaiter.Instance.SendCache(
                    $"GroupMemberInfo-{GroupNum}",
                    info.Uin.ToString(),
                    model,
                    DateTime.Now.AddDays(7));
            }

            return true;
        }
    }
}
