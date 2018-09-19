using System;
using System.Linq;
using Dolany.Ice.Ai.MahuaApis;
using Dolany.Ice.Ai.DolanyAI.Db;

namespace Dolany.Ice.Ai.DolanyAI
{
    public static class GroupMemberInfoCacher
    {
        public static MemberRoleCache GetMemberInfo(ReceivedMsgDTO MsgDTO)
        {
            using (var db = new AIDatabase())
            {
                var query = db.MemberRoleCache.Where(ic => ic.QQNum == MsgDTO.FromQQ &&
                                                           ic.GroupNum == MsgDTO.FromGroup);
                if (query.IsNullOrEmpty())
                {
                    return GetNewInfo(MsgDTO);
                }

                var Cache = query.First();
                if (Cache.Datatime.AddDays(7) < DateTime.Now)
                {
                    return GetNewInfo(MsgDTO);
                }

                return Cache.Clone();
            }
        }

        private static MemberRoleCache GetNewInfo(ReceivedMsgDTO MsgDTO)
        {
            using (var db = new AIDatabase())
            {
                var infos = AmandaAPIEx.GetMemberInfos(MsgDTO.FromGroup);
                if (infos == null)
                {
                    RuntimeLogger.Log($"Cannot get Group Member Infos:{MsgDTO.FromGroup}");
                    return null;
                }

                foreach (var info in infos.mems)
                {
                    var query = db.MemberRoleCache.Where(p => p.GroupNum == MsgDTO.FromGroup &&
                                                              p.QQNum == info.uin);
                    if (!query.IsNullOrEmpty())
                    {
                        var cache = query.First();
                        cache.Role = info.role;
                        cache.Datatime = DateTime.Now;
                        cache.Nickname = info.nick;
                    }
                    else
                    {
                        db.MemberRoleCache.Add(new MemberRoleCache
                        {
                            Id = Guid.NewGuid().ToString(),
                            Datatime = DateTime.Now,
                            GroupNum = MsgDTO.FromGroup,
                            Nickname = info.nick,
                            QQNum = info.uin,
                            Role = info.role
                        });
                    }
                }
                db.SaveChanges();

                return db.MemberRoleCache.First(i => i.QQNum == MsgDTO.FromQQ)
                                         .Clone();
            }
        }
    }
}