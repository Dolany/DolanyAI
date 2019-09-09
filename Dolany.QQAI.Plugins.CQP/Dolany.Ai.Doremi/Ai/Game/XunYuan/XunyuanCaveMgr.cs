using System.Collections.Generic;
using Dolany.Ai.Common;

namespace Dolany.Ai.Doremi.Ai.Game.XunYuan
{
    public class XunyuanCaveMgr
    {
        public static XunyuanCaveMgr Instance { get; } = new XunyuanCaveMgr();

        private readonly List<XunyuanCaveModel> Caves;

        public XunyuanCaveModel RandCaves => Caves.RandElement();

        public XunyuanCaveMgr()
        {
            Caves = CommonUtil.ReadJsonData_NamedList<XunyuanCaveModel>("XunyuanCaveData");
        }
    }
}
