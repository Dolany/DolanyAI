using System;
using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Cache;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Common
{
    public class GroupSettingSvc : IDependency
    {
        public static IEnumerable<GroupSettings> AllGroups =>
            RapidCacher.GetCache("AllGroups", TimeSpan.FromMinutes(10), () => MongoService<GroupSettings>.Get(p => !p.ForcedShutDown));

        public GroupSettings this[long GroupNum] => MongoService<GroupSettings>.GetOnly(g => g.GroupNum == GroupNum);

        public static void Delete(long GroupNum)
        {
            MongoService<GroupSettings>.DeleteMany(p => p.GroupNum == GroupNum);
        }
    }
}
