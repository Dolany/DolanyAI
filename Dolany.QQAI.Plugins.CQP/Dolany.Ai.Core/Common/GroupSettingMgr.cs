using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Common
{
    public class GroupSettingMgr
    {
        public static GroupSettingMgr Instance { get; } = new GroupSettingMgr();

        public Dictionary<long, GroupSettings> SettingDic;

        public GroupSettings this[long GroupNum] => SettingDic[GroupNum];

        private GroupSettingMgr()
        {
            Refresh();
        }

        public void Refresh()
        {
            SettingDic = MongoService<GroupSettings>.Get(p => !p.ForcedShutDown && p.BindAi == Configger.Instance["BindAi"])
                .Where(p => p.ExpiryTime != null && p.ExpiryTime.Value >= DateTime.Now)
                .ToDictionary(p => p.GroupNum, p => p);
        }
    }
}
