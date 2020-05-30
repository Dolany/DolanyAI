using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Common
{
    public class GroupSettingSvc : IDependency
    {
        public static IEnumerable<GroupSettings> AllGroups => MongoService<GroupSettings>.Get(p => !p.ForcedShutDown);

        public GroupSettings this[long GroupNum] => MongoService<GroupSettings>.GetOnly(p => !p.ForcedShutDown && p.GroupNum == GroupNum);

        public static void Delete(long GroupNum)
        {
            MongoService<GroupSettings>.DeleteMany(p => p.GroupNum == GroupNum);
        }
    }
}
