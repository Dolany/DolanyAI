using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.WorldLine.Standard.Ai.Game.Advanture
{
    public class CaveSettingHelper : IDataMgr
    {
        private List<CaveDataModel> CaveDatas;

        public static CaveSettingHelper Instance { get; } = new CaveSettingHelper();

        private CaveSettingHelper()
        {
            RefreshData();
            DataRefresher.Instance.Register(this);
        }

        public CaveDataModel GetCaveByName(string name)
        {
            return CaveDatas.FirstOrDefault(p => p.Name == name);
        }

        public CaveDataModel GetCaveByNo(int no)
        {
            return CaveDatas.FirstOrDefault(p => p.No == no);
        }

        public void RefreshData()
        {
            CaveDatas = CommonUtil.ReadJsonData_NamedList<CaveDataModel>("CaveSettingData");
        }
    }
}
