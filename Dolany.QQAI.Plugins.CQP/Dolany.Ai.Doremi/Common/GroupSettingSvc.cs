using System.Collections.Generic;
using System.Linq;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Ai.Doremi.Common
{
    public class GroupSettingSvc
    {
        public Dictionary<long, GroupSettings> SettingDic;

        public GroupSettings this[long GroupNum] => SettingDic[GroupNum];

        public GroupSettingSvc()
        {
            Refresh();
        }

        public void Refresh()
        {
            SettingDic = MongoService<GroupSettings>.Get(p => !p.ForcedShutDown).ToDictionary(p => p.GroupNum, p => p);
        }
    }
}
