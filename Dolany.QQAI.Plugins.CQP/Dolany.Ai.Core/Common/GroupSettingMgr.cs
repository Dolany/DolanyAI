using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Common
{
    public class GroupSettingMgr : IDataMgr
    {
        public static GroupSettingMgr Instance { get; } = new GroupSettingMgr();

        public Dictionary<long, GroupSettings> SettingDic;

        public GroupSettings this[long GroupNum] => SettingDic.ContainsKey(GroupNum) ? SettingDic[GroupNum] : null;

        private GroupSettingMgr()
        {
            RefreshData();
            DataRefresher.Instance.Register(this);
        }

        public void RefreshData()
        {
            SettingDic = MongoService<GroupSettings>.Get(p => !p.ForcedShutDown).ToDictionary(p => p.GroupNum, p => p);
        }
    }
}
