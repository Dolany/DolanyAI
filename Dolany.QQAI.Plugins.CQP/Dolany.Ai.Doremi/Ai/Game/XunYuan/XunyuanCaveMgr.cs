using System.Collections.Generic;
using Dolany.Ai.Common;

namespace Dolany.Ai.Doremi.Ai.Game.XunYuan
{
    public class XunyuanCaveMgr : IDataMgr
    {
        public static XunyuanCaveMgr Instance { get; } = new XunyuanCaveMgr();

        private List<XunyuanCaveModel> Caves;

        public XunyuanCaveModel RandCaves => Caves.RandElement();

        public XunyuanCaveMgr()
        {
            RefreshData();
            //DataRefresher.Instance.Register(this);
        }

        public void RefreshData()
        {
            Caves = CommonUtil.ReadJsonData_NamedList<XunyuanCaveModel>("XunyuanCaveData");
        }
    }
}
