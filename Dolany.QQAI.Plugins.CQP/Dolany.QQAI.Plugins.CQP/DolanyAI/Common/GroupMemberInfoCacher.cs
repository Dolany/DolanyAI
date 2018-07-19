using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newbe.Mahua;

namespace Dolany.QQAI.Plugins.CQP.DolanyAI
{
    public class GroupMemberInfoCacher
    {
        private static Dictionary<GroupMemberInfo, DateTime> InfoCache;

        public GroupMemberInfo GetMemberInfo(GroupMsgDTO MsgDTO)
        {
            if (InfoCache == null)
            {
                InfoCache = new Dictionary<GroupMemberInfo, DateTime>();
                var info = GetNewInfo(MsgDTO);
                InfoCache.Add(info, DateTime.Now);
                return info;
            }

            var query = InfoCache.Where(ic => long.Parse(ic.Key.Qq) == MsgDTO.FromQQ
                                        && long.Parse(ic.Key.Group) == MsgDTO.FromGroup);
            if (query.IsNullOrEmpty())
            {
                var info = GetNewInfo(MsgDTO);
                InfoCache.Add(info, DateTime.Now);
                return info;
            }

            var Cache = query.First();
            if (Cache.Value.AddDays(7) > DateTime.Now)
            {
                InfoCache.Remove(Cache.Key);
                var info = GetNewInfo(MsgDTO);
                InfoCache.Add(info, DateTime.Now);
                return info;
            }

            return Cache.Key;
        }

        private GroupMemberInfo GetNewInfo(GroupMsgDTO MsgDTO)
        {
            using (var robotSession = MahuaRobotManager.Instance.CreateSession())
            {
                var api = robotSession.MahuaApi;
                return api.GetGroupMemberInfo(MsgDTO.FromGroup.ToString(), MsgDTO.FromQQ.ToString());
            }
        }
    }
}