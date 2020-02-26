using System.Collections.Generic;
using Dolany.Ai.Common;

namespace Dolany.Ai.Doremi.Ai.Game.XunYuan
{
    public class XunyuanCaveMgr : IDataMgr
    {
        private List<XunyuanCaveModel> Caves;

        public XunyuanCaveModel RandCaves => Caves.RandElement();

        private static DataRefresher DataRefresher => AutofacSvc.Resolve<DataRefresher>();

        public XunyuanCaveMgr()
        {
            RefreshData();
            DataRefresher.Register(this);
        }

        public void RefreshData()
        {
            Caves = CommonUtil.ReadJsonData_NamedList<XunyuanCaveModel>("XunyuanCaveData");
        }
    }
}
