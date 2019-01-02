namespace Dolany.Ai.Core.Cache
{
    using System.Linq;

    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Db;

    public static class GroupMemberInfoCacher
    {
        public static MemberRoleCache GetMemberInfo(MsgInformationEx MsgDTO)
        {
            using (var db = new AIDatabase())
            {
                var query = db.MemberRoleCache.Where(ic => ic.QQNum == MsgDTO.FromQQ &&
                                                           ic.GroupNum == MsgDTO.FromGroup);
                if (query.IsNullOrEmpty())
                {
                    return null;
                }

                var Cache = query.First();

                return Cache.Clone();
            }
        }
    }
}
