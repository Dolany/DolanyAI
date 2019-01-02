namespace Dolany.Ai.Core.AITools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Timers;

    using Dolany.Ai.Core.API;
    using Dolany.Ai.Core.Cache;
    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Db;

    public class GroupMemberRefreshTool : IAITool
    {
        public void Work()
        {
            JobScheduler.Instance.Add(JobTimer.DairlyInterval, TimeUp);
        }

        private void TimeUp(object sender, ElapsedEventArgs e)
        {
            var info = Waiter.Instance.WaitForRelationId(
                new MsgCommand { Time = DateTime.Now, Command = AiCommand.GetGroups });
            if (info == null || string.IsNullOrEmpty(info.Msg))
            {
                RuntimeLogger.Log("Cannot Get Groups");
                return;
            }

            var groups = ParseGroups(info.Msg);
            foreach (var group in groups)
            {
                RefreshGroupInfo(group);

                Thread.Sleep(1000);
            }
        }

        private IEnumerable<long> ParseGroups(string groupStr)
        {
            // todo
            return null;
        }

        private void RefreshGroupInfo(long GroupNum)
        {
            using (var db = new AIDatabase())
            {
                var infos = APIEx.GetMemberInfos(GroupNum);
                if (infos == null)
                {
                    RuntimeLogger.Log($"Cannot get Group Member Infos:{GroupNum}");
                    return;
                }

                foreach (var info in infos.mems)
                {
                    var query = db.MemberRoleCache.Where(p => p.GroupNum == GroupNum &&
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
                                                       GroupNum = GroupNum,
                                                       Nickname = info.nick,
                                                       QQNum = info.uin,
                                                       Role = info.role
                                                   });
                    }
                }
                db.SaveChanges();
            }
        }
    }
}
