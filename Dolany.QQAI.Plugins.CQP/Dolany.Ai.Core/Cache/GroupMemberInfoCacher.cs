using System.Linq;
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

            var setting = GroupSettingMgr.Instance[GroupNum];
            setting.AuthInfo = new GroupAuthInfoModel {Owner = infos.owner, Mgrs = infos.adm.ToList()};

            setting.Update();
            GroupSettingMgr.Instance.Refresh();
            Logger.Log($"Refresh Group Info: {GroupNum} completed");

            return true;
        }
    }
}
