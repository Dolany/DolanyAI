using System.Collections.Generic;
using Dolany.Ai.Common;

namespace Dolany.Ai.Doremi.Ai.Game.XunYuan
{
    public class XunyuanCaveSvc : IDataMgr
    {
        private List<XunyuanCaveModel> Caves;

        public XunyuanCaveModel RandCaves => Caves.RandElement();

        private static DataRefreshSvc DataRefreshSvc => AutofacSvc.Resolve<DataRefreshSvc>();

        public XunyuanCaveSvc()
        {
            RefreshData();
            DataRefreshSvc.Register(this);
        }

        public void RefreshData()
        {
            Caves = CommonUtil.ReadJsonData_NamedList<XunyuanCaveModel>("XunyuanCaveData");
        }
    }
}
