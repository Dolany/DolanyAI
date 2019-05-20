using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Ai.Game.Advanture
{
    public class CaveSettingHelper
    {
        private readonly List<CaveDataModel> CaveDatas;

        public static CaveSettingHelper Instance { get; } = new CaveSettingHelper();

        private CaveSettingHelper()
        {
            CaveDatas = CommonUtil.ReadJsonData_NamedList<CaveDataModel>("CaveSettingData");
        }

        public CaveDataModel GetCaveByName(string name)
        {
            return CaveDatas.FirstOrDefault(p => p.Name == name);
        }

        public CaveDataModel GetCaveByNo(int no)
        {
            return CaveDatas.FirstOrDefault(p => p.No == no);
        }
    }
}
