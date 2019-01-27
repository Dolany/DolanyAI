using Dolany.Database.Sqlite;

namespace Dolany.Ai.Core.Cache
{
    using System;
    using System.Linq;

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
            if (infos?.members == null)
            {
                RuntimeLogger.Log($"Cannot get Group Member Infos:{GroupNum}");
                return false;
            }

            foreach (var info in infos.members)
            {
                var qqnum = long.Parse(info.Key);
                var role = 2;
                if (infos.owner == qqnum)
                {
                    role = 0;
                }
                else if (infos.adm != null && infos.adm.Contains(qqnum))
                {
                    role = 1;
                }
                var model = new MemberRoleCache
                                {
                                    GroupNum = GroupNum,
                                    Nickname = info.Value.nk,
                                    QQNum = qqnum,
                                    Role = role
                                };
                SCacheService.Cache($"GroupMemberInfo-{GroupNum}-{qqnum}", model, DateTime.Now.AddDays(7));
            }
            RuntimeLogger.Log($"Refresh Group Info: {GroupNum} completed");

            return true;
        }
    }
}
