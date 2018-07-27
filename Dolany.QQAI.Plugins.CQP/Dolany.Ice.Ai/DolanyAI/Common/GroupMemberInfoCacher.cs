using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newbe.Mahua;
using Dolany.Ice.Ai.MahuaApis;
using Dolany.Ice.Ai.DolanyAI.Db;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class GroupMemberInfoCacher
    {
        public MemberRoleCache GetMemberInfo(GroupMsgDTO MsgDTO)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.MemberRoleCache.Where(ic => ic.QQNum == MsgDTO.FromQQ
                                        && ic.GroupNum == MsgDTO.FromGroup);
                if (query.IsNullOrEmpty())
                {
                    var info = GetNewInfo(MsgDTO);
                    return info;
                }

                var Cache = query.First();
                if (Cache.Datatime.AddDays(7) < DateTime.Now)
                {
                    var info = GetNewInfo(MsgDTO);
                    return info;
                }

                return Cache.Clone();
            }
        }

        private MemberRoleCache GetNewInfo(GroupMsgDTO MsgDTO)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var infos = AmandaAPIEx.GetMemberInfos(MsgDTO.FromGroup);
                foreach (var info in infos.mems)
                {
                    if (db.MemberRoleCache.Any(p => p.GroupNum == MsgDTO.FromGroup && p.QQNum == info.uin))
                    {
                        var cache = db.MemberRoleCache.Where(p => p.GroupNum == MsgDTO.FromGroup && p.QQNum == info.uin).First();
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

                return db.MemberRoleCache.First(i => i.QQNum == MsgDTO.FromQQ).Clone();
            }
        }
    }
}