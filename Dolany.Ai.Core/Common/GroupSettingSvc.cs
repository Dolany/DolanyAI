using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Common
{
    public class GroupSettingSvc : IDataMgr, IDependency
    {
        public Dictionary<long, GroupSettings> SettingDic;

        public GroupSettings this[long GroupNum] => SettingDic.ContainsKey(GroupNum) ? SettingDic[GroupNum] : null;

        public void RefreshData()
        {
            SettingDic = MongoService<GroupSettings>.Get(p => !p.ForcedShutDown).ToDictionary(p => p.GroupNum, p => p);
        }
    }
}
