namespace Dolany.Ai.Core.Cache
{
    using System.Linq;

    using API;

    using Common;

    using Dolany.Database.Ai;

    public class GroupMemberInfoCacher
    {
        public static bool RefreshGroupInfo(long GroupNum)
        {
            var infos = APIEx.GetMemberInfos(GroupNum);
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
