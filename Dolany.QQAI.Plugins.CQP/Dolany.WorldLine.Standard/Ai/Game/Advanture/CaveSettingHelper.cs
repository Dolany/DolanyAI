using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.WorldLine.Standard.Ai.Game.Advanture
{
    public class CaveSettingHelper : IDataMgr, IDependency
    {
        private List<CaveDataModel> CaveDatas;

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
