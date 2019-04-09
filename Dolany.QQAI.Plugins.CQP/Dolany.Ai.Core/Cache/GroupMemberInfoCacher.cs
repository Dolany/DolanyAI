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
            setting.AuthInfo = new GroupAuthInfoModel();

            foreach (var (key, _) in infos.members)
            {
                var qqnum = long.Parse(key);
                if (infos.owner == qqnum)
                {
                    setting.AuthInfo.Owner = qqnum;
                    continue;
                }

                if (infos.adm != null && infos.adm.Contains(qqnum))
                {
                    setting.AuthInfo.Mgrs.Add(qqnum);
                }
            }
            setting.Update();
            GroupSettingMgr.Instance.Refresh();
            Logger.Log($"Refresh Group Info: {GroupNum} completed");

            return true;
        }
    }
}
