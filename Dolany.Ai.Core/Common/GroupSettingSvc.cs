using System;
using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Cache;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Common
{
    /// <summary>
    /// 群组设定服务
    /// </summary>
    public class GroupSettingSvc : IDependency
    {
        /// <summary>
        /// 所有群组信息
        /// </summary>
        public static IEnumerable<GroupSettings> AllGroups => MongoService<GroupSettings>.Get(p => !p.ForcedShutDown);

        /// <summary>
        /// 获取指定的群组信息
        /// </summary>
        /// <param name="GroupNum"></param>
        public GroupSettings this[long GroupNum] => MongoService<GroupSettings>.GetOnly(g => g.GroupNum == GroupNum);

        /// <summary>
        /// 移除一个群组信息
        /// </summary>
        /// <param name="GroupNum"></param>
        public static void Delete(long GroupNum)
        {
            MongoService<GroupSettings>.DeleteMany(p => p.GroupNum == GroupNum);
        }
    }
}
