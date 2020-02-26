using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.API;
using Dolany.Ai.Core.Common;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Cache
{
    public class GroupMemberInfoCacher
    {
        public static bool RefreshGroupInfo(long GroupNum, string BindAi)
        {
            var infos = APIEx.GetMemberInfos(GroupNum, BindAi);
            if (infos?.members == null)
            {
                Logger.Log($"Cannot get Group Member Infos:{GroupNum}");
                return false;
            }

            var GroupSettingMgr = AutofacSvc.Resolve<GroupSettingMgr>();
            var setting = GroupSettingMgr[GroupNum];
            setting.AuthInfo = new GroupAuthInfoModel {Owner = infos.owner, Mgrs = infos.adm?.ToList() ?? new List<long>()};
            setting.MembersCount = infos.members.Count;

            setting.Update();
            GroupSettingMgr.RefreshData();
            Logger.Log($"Refresh Group Info: {GroupNum} completed");

            return true;
        }
    }
}
