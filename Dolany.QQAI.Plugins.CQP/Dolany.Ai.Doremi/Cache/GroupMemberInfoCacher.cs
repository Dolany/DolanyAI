using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Doremi.API;
using Dolany.Ai.Doremi.Common;
using Dolany.Database.Ai;

namespace Dolany.Ai.Doremi.Cache
{
    public class GroupMemberInfoCacher
    {
        private static GroupSettingMgr GroupSettingMgr => AutofacSvc.Resolve<GroupSettingMgr>();

        public static bool RefreshGroupInfo(long GroupNum, string BindAi)
        {
            var infos = APIEx.GetMemberInfos(GroupNum, BindAi);
            if (infos?.members == null)
            {
                Logger.Log($"Cannot get Group Member Infos:{GroupNum}");
                return false;
            }

            var setting = GroupSettingMgr[GroupNum];
            setting.AuthInfo = new GroupAuthInfoModel {Owner = infos.owner, Mgrs = infos.adm?.ToList() ?? new List<long>()};

            setting.Update();
            GroupSettingMgr.Refresh();
            Logger.Log($"Refresh Group Info: {GroupNum} completed");

            return true;
        }
    }
}
