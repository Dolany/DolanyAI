using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.UtilityTool;

namespace Dolany.WorldLine.Doremi.Ai.Game.XunYuan
{
    public class XunyuanCaveSvc : IDataMgr, IDependency
    {
        private List<XunyuanCaveModel> Caves;

        public XunyuanCaveModel RandCaves => Caves.RandElement();

        public void RefreshData()
        {
            Caves = CommonUtil.ReadJsonData_NamedList<XunyuanCaveModel>("Doremi/XunyuanCaveData");
        }
    }
}
