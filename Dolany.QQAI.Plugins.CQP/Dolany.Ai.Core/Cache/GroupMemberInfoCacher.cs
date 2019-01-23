using Dolany.Database.Sqlite;

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
            var cacheResponse =
                SCacheService.Get<MemberRoleCache>($"GroupMemberInfo-{MsgDTO.FromGroup}-{MsgDTO.FromQQ}");

            if (cacheResponse != null)
            {
                return cacheResponse;
            }

            const string key = "AuthDisable";
            var cache = SCacheService.Get<string>(key);
            if (string.IsNullOrEmpty(cache))
            {
                RefreshGroupInfo(MsgDTO.FromGroup);
            }

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
                                    Nickname = info.Nick,
                                    QQNum = info.Uin,
                                    Role = info.Role
                                };
                SCacheService.Cache($"GroupMemberInfo-{GroupNum}-{info.Uin}", model, DateTime.Now.AddDays(7));
            }

            return true;
        }
    }
}
